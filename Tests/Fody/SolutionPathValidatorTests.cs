using System;
using System.IO;
using Fody;
using Moq;
using Xunit;

public class SolutionPathValidatorTests : TestBase
{
    [Fact]
    public void Valid()
    {
        var loggerMock = new Mock<BuildLogger>();

        loggerMock.Setup(x => x.LogDebug(It.Is<string>(y => y.Contains(Environment.CurrentDirectory))));
        var buildLogger = loggerMock.Object;

        var processor = new Processor
        {
            Logger = buildLogger,
            SolutionDirectory = Environment.CurrentDirectory
        };
        processor.ValidateSolutionPath();
        loggerMock.Verify();
    }

    [Fact]
    public void InValid()
    {
        Action paramName = () =>
        {
            var processor = new Processor
            {
                SolutionDirectory = "aString"
            };
            processor.ValidateSolutionPath();
        };
#pragma warning disable xUnit2015 // Do not use typeof expression to check the exception type
        var exception = Assert.Throws(typeof(WeavingException), paramName);
#pragma warning restore xUnit2015 // Do not use typeof expression to check the exception type
        Assert.Equal($"SolutionDir '{Path.GetFullPath("aString")}' does not exist.", exception.Message);
    }
}