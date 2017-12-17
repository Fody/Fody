using Xunit;
using WithStrongName;

public class WithStrongNameTest
{
    [Fact]
    public void EnsureIsStrongNamed()
    {
        var assemblyName = typeof(Class1).Assembly.GetName();
        var publicKeyToken = assemblyName.GetPublicKeyToken();
        Assert.NotEmpty(publicKeyToken);
    }
}