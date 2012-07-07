using System;
using Fody;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SolutionPathValidatorTests
{
    [Test]
    public void Valid()
    {
        var buildLogger = Substitute.For<BuildLogger>();
        var weavingTask = new WeavingTask
                              {
                                  SolutionDir = Environment.CurrentDirectory
                              };

        var pathValidator = new SolutionPathValidator
                                {
                                    Logger = buildLogger,
                                    WeavingTask = weavingTask
                                };
        pathValidator.Execute();
        buildLogger.Received(1).LogInfo(Arg.Is<string>(x => x.Contains(Environment.CurrentDirectory)));
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(WeavingException),ExpectedMessage = "SolutionDir \"\" does not exist.")]
    public void InValid()
    {
        var weavingTask = new WeavingTask();

        var pathValidator = new SolutionPathValidator
                                {
                                    WeavingTask = weavingTask
                                };
        pathValidator.Execute();
    }
}