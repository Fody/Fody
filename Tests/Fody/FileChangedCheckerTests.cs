using Moq;
using Xunit;

public class FileChangedCheckerTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var processor = new Processor
        {
            ContainsTypeChecker = new Mock<ContainsTypeChecker>().Object,
            Logger = new Mock<BuildLogger>().Object,
            AssemblyFilePath = GetType().Assembly.Location
        };

        Assert.False(processor.TargeAssemblyHasAlreadyBeenProcessed());
    }
}