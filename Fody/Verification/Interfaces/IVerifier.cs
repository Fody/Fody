namespace Fody.Verification
{
    public interface IVerifier
    {
        string Name { get; }
        bool Verify(string assemblyFileName);
    }
}