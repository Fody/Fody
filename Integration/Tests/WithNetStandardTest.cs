using NUnit.Framework;
using WithNetStandard;

[TestFixture]
public class WithNetStandardTest
{
    [Test]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.IsTrue(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}