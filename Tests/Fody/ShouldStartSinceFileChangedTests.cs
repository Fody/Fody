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
            AssemblyFilePath = typeof(string).Assembly.CodeBase.Replace("file:///", "")
        };
        Assert.False(processor.TargetAssemblyHasAlreadyBeenProcessed());
    }

    [Fact]
    public void ProcessedByFody()
    {
        var processor = new Processor
        {
            Logger = new Mock<BuildLogger>().Object,
            AssemblyFilePath = GetType().Assembly.CodeBase.Replace("file:///", "")
        };
        Assert.True(processor.TargetAssemblyHasAlreadyBeenProcessed());
    }
}

#pragma warning disable IDE1006 // Naming Styles
public interface ProcessedByFody
#pragma warning restore IDE1006 // Naming Styles
{
    // this type just emulates that this assembly looks like it has been processed by Fody.
}