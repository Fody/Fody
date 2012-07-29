using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public Action<string> LogInfo { get; set; }
    public Action<string, SequencePoint> LogWarningPoint { get; set; }
    public Action<string, SequencePoint> LogErrorPoint { get; set; }

    public void Execute()
    {
        LogInfo("Hello");
        var class1 = ModuleDefinition.Types.First(x => x.Name == "Class1");
        var method1 = class1.Methods.First(x => x.Name == "Method1");
        var sequencePoints = method1.Body.Instructions
            .Select(x => x.SequencePoint)
            .Where(x => x != null).ToList();

        LogWarningPoint("Nav to sequencePoint", sequencePoints[1]);
        ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public, ModuleDefinition.Import(typeof(object))));
    }
}