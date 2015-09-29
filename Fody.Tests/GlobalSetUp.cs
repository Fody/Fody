using NUnit.Framework;

[SetUpFixture]
public class GlobalSetUp
{
    [SetUp]
    void Setup()
    {
        AssemblyLocation.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    }

}