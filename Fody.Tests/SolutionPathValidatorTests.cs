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

       loggerMock.Setup(x=>x.LogInfo(It.Is<string>(y => y.Contains(Environment.CurrentDirectory))));
        var buildLogger = loggerMock.Object;

        var pathValidator = new Processor
                                {
                                    Logger = buildLogger,
                                    SolutionDir = Environment.CurrentDirectory
                                };
        pathValidator.ValidateSolutionPath();
        loggerMock.Verify();
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(WeavingException), ExpectedMessage = "SolutionDir \"baddir\" does not exist.")]
    public void InValid()
    {
        var pathValidator = new Processor
            {
                SolutionDir = "baddir"
            };
        pathValidator.ValidateSolutionPath();
    }
}