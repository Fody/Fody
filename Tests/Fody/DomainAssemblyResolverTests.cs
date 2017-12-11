using Xunit;

public class DomainAssemblyResolverTests : TestBase
{
    [Fact]
    public void GetAssembly()
    {
        Assert.NotNull(DomainAssemblyResolver.GetAssembly(GetType().Assembly.GetName().FullName));
    }
}