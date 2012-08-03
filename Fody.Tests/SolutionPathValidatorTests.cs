using System;
using Moq;
using NUnit.Framework;

[TestFixture]
public class SolutionPathValidatorTests
{
    [Test]
    public void Valid()
    {
        var loggerMock = new Mock<BuildLogger>();

        loggerMock.Setup(x => x.LogInfo(It.Is<string>(y => y.Contains(Environment.CurrentDirectory))));
        var buildLogger = loggerMock.Object;

        var processor = new Processor
            {
                Logger = buildLogger,
                SolutionDir = Environment.CurrentDirectory
            };
        processor.ValidateSolutionPath();
        loggerMock.Verify();
    }

    [Test]
    [ExpectedException(ExpectedException = typeof (WeavingException), ExpectedMessage = "SolutionDir \"baddir\" does not exist.")]
    public void InValid()
    {
        var processor = new Processor
            {
                SolutionDir = "baddir"
            };
        processor.ValidateSolutionPath();
    }
}