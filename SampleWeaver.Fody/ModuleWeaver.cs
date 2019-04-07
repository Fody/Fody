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
        VerifySymbols();

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

    void VerifySymbols()
    {
        LogInfo("Verify Symbols");

        var anyMethod = ModuleDefinition.GetTypes()
            .Where(type => type.IsClass)
            .SelectMany(type => type.GetMethods())
            .FirstOrDefault();

        if (anyMethod == null)
        {
            LogInfo("Assembly has no type with a method, symbol verifying skipped");
            return;
        }

        var shouldHaveSymbols = ModuleDefinition.Assembly.CustomAttributes.All(attr => attr.AttributeType.Name != "NoSymbolsMarkerAttribute");
        LogInfo("Assembly should have symbols: " + shouldHaveSymbols);
        var hasSymbols = HasSymbols(anyMethod);
        LogInfo("Assembly has symbols: " + hasSymbols);

        if (shouldHaveSymbols != hasSymbols)
        {
            LogError($"Unexpected symbols in assembly {ModuleDefinition.FileName}, should have: {shouldHaveSymbols}, but has: {hasSymbols}");
        }
    }

    bool HasSymbols(MethodDefinition method)
    {
        try
        {
            return ModuleDefinition.SymbolReader.Read(method).HasSequencePoints;
        }
        catch
        {
            return false;
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }

    public override bool ShouldCleanReference => false;
}