using System.IO;

public class SolutionPathValidatorTests
{
    [Fact]
    public void Valid()
    {
        var loggerMock = new MockBuildLogger();

        var processor = new Processor
        {
            Logger = loggerMock,
            SolutionDirectory = Environment.CurrentDirectory
        };
        processor.ValidateSolutionPath();
    }

    [Fact]
    public void InValid()
    {
        Action action = () =>
        {
            var processor = new Processor
            {
                SolutionDirectory = "aString"
            };
            processor.ValidateSolutionPath();
        };
        var exception = Assert.ThrowsAny<Exception>(action);
        Assert.Equal($"SolutionDir '{Path.GetFullPath("aString")}' does not exist.", exception.Message);
    }
}