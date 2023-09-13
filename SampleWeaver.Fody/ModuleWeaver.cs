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

        RuntimeCopyLocalPaths.Remove(filePath);
        RuntimeCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".pdb"));
        RuntimeCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".xml"));

        // Do not use ShouldCleanReference in order to test the above code
        var assemblyRef = ModuleDefinition.AssemblyReferences.FirstOrDefault(_ => _.Name == "SampleWeaver");
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
            LogInfo("Validating method " + methodInfo.Method.FullName);

            var shouldHaveSymbols = methodInfo.Attribute.GetPropertyValue(SymbolValidationAttributePropertyName, true);
            LogInfo("Assembly should have symbols: " + shouldHaveSymbols);

            var hasSymbols = HasSymbols(methodInfo.Method);
            LogInfo("Assembly has symbols: " + hasSymbols);

            if (shouldHaveSymbols != hasSymbols)
            {
                LogError($"Unexpected symbols in assembly {ModuleDefinition.FileName}, should have: {shouldHaveSymbols}, but has: {hasSymbols}");
            }
        }
    }

    IEnumerable<MethodInfos> GetMethodInfos(string symbolValidationAttributeTypeName) =>
        from type in ModuleDefinition.GetTypes()
        where type.IsClass
        from method in type.GetMethods()
        let attribute = method.ConsumeAttribute(symbolValidationAttributeTypeName)
        where attribute != null
        select new MethodInfos(method, attribute);

    bool HasSymbols(MethodDefinition method) =>
        ModuleDefinition.SymbolReader?.Read(method)?.HasSequencePoints == true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }

    // Do not use ShouldCleanReference in order to test the above code
    public override bool ShouldCleanReference => false;
}

class MethodInfos
{
    public MethodDefinition Method { get; }
    public CustomAttribute Attribute { get; }

    public MethodInfos(MethodDefinition method, CustomAttribute attribute)
    {
        Method = method;
        Attribute = attribute;
    }
}

static class AttributeExtensionMethods
{
    public static CustomAttribute? ConsumeAttribute(this ICustomAttributeProvider attributeProvider, string attributeName)
    {
        var attributes = attributeProvider.CustomAttributes;
        var matches = attributes.Where(attribute => attribute.Constructor.DeclaringType.FullName == attributeName).ToList();

        foreach (var match in matches)
        {
            attributes.Remove(match);
        }

        return matches.FirstOrDefault();
    }

    public static T GetPropertyValue<T>(this CustomAttribute attribute, string propertyName, T defaultValue) =>
        attribute.Properties.Where(_ => _.Name == propertyName)
            .Select(p => (T)p.Argument.Value)
            .DefaultIfEmpty(defaultValue)
            .Single();
}
