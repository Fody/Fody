using System;
using Mono.Cecil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public Action<string> LogInfo { get; set; }

    public void Execute()
    {
        LogInfo("Hello");
        ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public, ModuleDefinition.Import(typeof(object))));
    }
}