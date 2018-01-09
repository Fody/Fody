using Mono.Cecil;


public class NonModifyingFakeModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public bool IsAssemblyUnmodified { get; } = true;

    public void Execute()
    {
    }

    public void AfterWeaving()
    {
    }
}