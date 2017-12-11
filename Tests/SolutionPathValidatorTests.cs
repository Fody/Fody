using System;
using System.IO;
using Moq;
using NUnit.Framework;

[TestFixture]
public class SolutionPathValidatorTests
{
    [Test]
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

    [Test]
    public void InValid()
    {
        Assert.Throws<WeavingException>(() =>
        {
            var processor = new Processor
            {
                SolutionDirectory = "aString"
            };
            processor.ValidateSolutionPath();
        }, $"SolutionDir \"{Path.GetFullPath("baddir")}aString\" does not exist.");
    }
}