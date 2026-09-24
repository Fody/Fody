using System.IO;

public class SolutionPathValidatorTests
{
    [Test]
    public async Task Valid()
    {
        var loggerMock = new MockBuildLogger();

        var processor = new Processor
        {
            Logger = loggerMock,
            SolutionDirectory = Environment.CurrentDirectory
        };
        processor.ValidateSolutionPath();
    }

    [Test]
    public async Task InValid()
    {
        Action action = () =>
        {
            var processor = new Processor
            {
                SolutionDirectory = "aString"
            };
            processor.ValidateSolutionPath();
        };
        var exception = await Assert.That(action).ThrowsException();
        await Assert.That(exception!.Message).IsEqualTo($"SolutionDir '{Path.GetFullPath("aString")}' does not exist.");
    }
}