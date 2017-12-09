using NUnit.Framework;

[TestFixture]
public class DomainAssemblyResolverTests
{
    [Test]
    public void GetAssembly()
    {
        Assert.IsNotNull( DomainAssemblyResolver.GetAssembly(GetType().Assembly.GetName().FullName));
    }
}