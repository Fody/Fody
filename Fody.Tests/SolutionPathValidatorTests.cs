using System;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SolutionPathValidatorTests
{
    [Test]
    public void Valid()
    {
        var buildLogger = Substitute.For<BuildLogger>();

        var pathValidator = new SolutionPathValidator
                                {
                                    Logger = buildLogger,
                                    SolutionDir = Environment.CurrentDirectory
                                };
        pathValidator.Execute();
        buildLogger.Received(1).LogInfo(Arg.Is<string>(x => x.Contains(Environment.CurrentDirectory)));
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(WeavingException), ExpectedMessage = "SolutionDir \"\" does not exist.")]
    public void InValid()
    {
        var pathValidator = new SolutionPathValidator
            {
                SolutionDir = "baddir"
            };
        pathValidator.Execute();
    }
}