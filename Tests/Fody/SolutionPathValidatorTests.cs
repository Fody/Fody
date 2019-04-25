using System;
using System.IO;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class SolutionPathValidatorTests :
    XunitLoggingBase
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

    public SolutionPathValidatorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}