using NUnit.Framework;

[TestFixture]
public class ShouldStartSinceFileChangedTests
{
    [Test]
    public void Simple()
    {
        var processor = new Processor
            {
                AssemblyPath = GetType().Assembly.CodeBase.Replace("file:///", "")
            };
        Assert.IsTrue(processor.ShouldStartSinceFileChanged());
    }
}