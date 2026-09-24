using WithNoPdb;

public class WithNoPdbTest
{
    [Test]
    public async Task EnsureTypeChangedByNugetWeaver()
    {
        await Assert.That(typeof(Class1).GetMethod("Method").IsVirtual).IsTrue();
    }
}