using Moq;
using Xunit;

public class ShouldStartSinceFileChangedTests : TestBase
{
    [Fact]
    public void NotProcessedByFody()
    {
        var processor = new Processor
            {
                Logger = new Mock<BuildLogger>().Object,
                AssemblyFilePath = typeof (string).Assembly.CodeBase.Replace("file:///", "")
            };
        Assert.True(processor.ShouldStartSinceFileChanged());
    }

    [Fact]
    public void ProcessedByFody()
    {
        var processor = new Processor
            {
                Logger = new Mock<BuildLogger>().Object,
                AssemblyFilePath = GetType().Assembly.CodeBase.Replace("file:///", "")
            };
        Assert.False(processor.ShouldStartSinceFileChanged());
    }
}

public interface ProcessedByFody
{

}