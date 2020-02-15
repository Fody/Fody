using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver :
    BaseModuleWeaver
{
    public override void Execute()
    {
        ValidateSymbols();

        var type = new TypeDefinition("SampleWeaverTest", "Configuration", TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.AutoClass | TypeAttributes.AnsiClass, TypeSystem.ObjectReference);
        var contentField = new FieldDefinition("Content", FieldAttributes.Public | FieldAttributes.Static, TypeSystem.StringReference);
        var propertyField = new FieldDefinition("PropertyValue", FieldAttributes.Public | FieldAttributes.Static, TypeSystem.StringReference);
        var method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, TypeSystem.VoidReference);

        var instructions = method.Body.Instructions;
        instructions.Add(Instruction.Create(OpCodes.Ldstr, Config?.ToString() ?? "Missing"));
        instructions.Add(Instruction.Create(OpCodes.Stsfld, contentField));
        instructions.Add(Instruction.Create(OpCodes.Ldstr, Config?.Attribute("MyProperty")?.Value ?? "Missing"));
        instructions.Add(Instruction.Create(OpCodes.Stsfld, propertyField));
        instructions.Add(Instruction.Create(OpCodes.Ret));

        type.Fields.Add(contentField);
        type.Fields.Add(propertyField);
        type.Methods.Add(method);
        ModuleDefinition.Types.Add(type);

        var intermediateFolder = Path.GetDirectoryName(ModuleDefinition.FileName);
        var additionalFilePath = Path.Combine(intermediateFolder, "SomeExtraFile.txt");

        File.WriteAllText(additionalFilePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        ReferenceCopyLocalPaths.Add(additionalFilePath);

        var customAttributes = ModuleDefinition.Assembly.CustomAttributes;

        var sampleAttr = customAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "SampleAttribute");
        if (sampleAttr == null)
        {
            return;
        }

        customAttributes.Remove(sampleAttr);
        var filePath = sampleAttr.AttributeType.Resolve().Module.FileName;

        ReferenceCopyLocalPaths.Remove(filePath);
        ReferenceCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".pdb"));
        ReferenceCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".xml"));

        // Do not use ShouldCleanReference in order to test the above code
        var assemblyRef = ModuleDefinition.AssemblyReferences.FirstOrDefault(i => i.Name == "SampleWeaver");
        if (assemblyRef != null)
        {
            ModuleDefinition.AssemblyReferences.Remove(assemblyRef);
        }
    }

    void ValidateSymbols()
    {
        const string SymbolValidationAttributeTypeName = "SampleWeaver.SymbolValidationAttribute";
        const string SymbolValidationAttributePropertyName = "HasSymbols";

        LogInfo("Validate Symbols");

        var methodInfos = GetMethodInfos(SymbolValidationAttributeTypeName).ToList();

        if (!methodInfos.Any())
        {
            LogInfo("Assembly has no method with a [SymbolValidation] attribute, symbol validation skipped");
            return;
        }

        foreach (var methodInfo in methodInfos)
        {
            LogInfo("Validating method " + methodInfo.method.FullName);

            var shouldHaveSymbols = methodInfo.attribute.GetPropertyValue(SymbolValidationAttributePropertyName, true);
            LogInfo("Assembly should have symbols: " + shouldHaveSymbols);

            var hasSymbols = HasSymbols(methodInfo.method);
            LogInfo("Assembly has symbols: " + hasSymbols);

            if (shouldHaveSymbols != hasSymbols)
            {
                LogError($"Unexpected symbols in assembly {ModuleDefinition.FileName}, should have: {shouldHaveSymbols}, but has: {hasSymbols}");
            }
        }
    }

    IEnumerable<(MethodDefinition method, CustomAttribute attribute)> GetMethodInfos(string SymbolValidationAttributeTypeName)
    {
        return from type in ModuleDefinition.GetTypes()
                .Where(x => x.IsClass)
            from method in type.GetMethods()
            let attribute = method.GetAttribute(SymbolValidationAttributeTypeName)
            where attribute != null
            select (method, attribute);
    }

    bool HasSymbols(MethodDefinition method)
    {
        return ModuleDefinition.SymbolReader?.Read(method)?.HasSequencePoints == true;
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }

    public override bool ShouldCleanReference => false;
}

static class AttributeExtensionMethods
{
    public static CustomAttribute? GetAttribute(this ICustomAttributeProvider? attributeProvider, string attributeName)
    {
        return attributeProvider?.CustomAttributes.GetAttribute(attributeName);
    }

    public static CustomAttribute? GetAttribute(this IEnumerable<CustomAttribute>? attributes, string attributeName)
    {
        return attributes?.FirstOrDefault(attribute => attribute.Constructor.DeclaringType.FullName == attributeName);
    }

    public static T GetPropertyValue<T>(this CustomAttribute attribute, string propertyName, T defaultValue)
    {
        return attribute.Properties.Where(p => p.Name == propertyName)
            .Select(p => (T)p.Argument.Value)
            .DefaultIfEmpty(defaultValue)
            .Single();
    }
}
