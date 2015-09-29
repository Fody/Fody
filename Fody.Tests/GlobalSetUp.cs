using NUnit.Framework;

[SetUpFixture]
public class GlobalSetUp
{
    [SetUp]
    public void Setup()
    {
        AssemblyLocation.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }

}