using Microsoft.Build.Framework;
using NUnit.Framework;

[TestFixture]
public class BuildLoggerTests
{
    [Test]
    public void NullMessageImportance()
    {
        var logger = new BuildLogger(null);
        Assert.AreEqual(MessageImportance.Low,logger.MessageImportance);
    }
    [Test]
    public void WhiteSpaceMessageImportance()
    {
        var logger = new BuildLogger("  ");
        Assert.AreEqual(MessageImportance.Low,logger.MessageImportance);
    }
    [Test]
    public void EmptyStringMessageImportance()
    {
        var logger = new BuildLogger(null);
        Assert.AreEqual(MessageImportance.Low,logger.MessageImportance);
    }
    [Test]
    public void MessageImportanceMessageImportance()
    {
        var logger = new BuildLogger("Normal");
        Assert.AreEqual(MessageImportance.Normal, logger.MessageImportance);
    }
}