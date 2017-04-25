using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public void Dispose()
    {
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        throw new System.NotImplementedException();
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        throw new System.NotImplementedException();
    }
}