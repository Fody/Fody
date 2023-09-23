using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public void Dispose()
    {
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name) =>
        throw new NotImplementedException();

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) =>
        throw new NotImplementedException();
}