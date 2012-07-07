using System;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    // Will contain the full element XML from FodyWeavers.xml. OPTIONAL
    public XElement Config { get; set; }

    // Will log an informational message to MSBuild. OPTIONAL
    public Action<string> LogInfo  { get; set; }

    // Will log an warning message to MSBuild. OPTIONAL
    public Action<string> LogWarning  { get; set; }

    // An instance of Mono.Cecil.IAssemblyResolver for resolving assembly references. OPTIONAL
    public IAssemblyResolver AssemblyResolver { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing. REQUIRED
    public ModuleDefinition ModuleDefinition { get; set; }

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogWarning = s => { };
        LogInfo = s => { };
    } 

    public void Execute()
    {
        ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public));
    }
}