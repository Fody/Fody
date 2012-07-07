using System;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class WeaversXmlHistoryTests
{
    [Test]
    public void AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var projectWeaversFinder = new ProjectWeaversFinder();
            projectWeaversFinder.ConfigFiles.Add(fileName);
            var xmlHistory = new WeaversXmlHistory
                                 {
                                     ProjectWeaversFinder = projectWeaversFinder
                                 };
            xmlHistory.CheckForChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), WeaversXmlHistory.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            WeaversXmlHistory.TimeStamps.Clear();
        }
    }
    [Test]
    public void AddExistingFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var projectWeaversFinder = new ProjectWeaversFinder();
            projectWeaversFinder.ConfigFiles.Add(fileName);
            var xmlHistory = new WeaversXmlHistory
                                 {
                                     ProjectWeaversFinder = projectWeaversFinder
                                 };
            xmlHistory.CheckForChanged();
            xmlHistory.CheckForChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), WeaversXmlHistory.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            WeaversXmlHistory.TimeStamps.Clear();
        }
    }

    [Test]
    public void AddChangedFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var expected = File.GetLastWriteTimeUtc(fileName);
            var logger = Substitute.For<BuildLogger>();
            var projectWeaversFinder = new ProjectWeaversFinder();
            projectWeaversFinder.ConfigFiles.Add(fileName);
            var xmlHistory = new WeaversXmlHistory
                                 {
                                     ProjectWeaversFinder = projectWeaversFinder,
                                     Logger = logger 
                                 };
            xmlHistory.CheckForChanged();
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            xmlHistory.CheckForChanged();

            logger.Received(1).LogWarning(Arg.Any<string>());

            Assert.AreEqual(expected, WeaversXmlHistory.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            WeaversXmlHistory.TimeStamps.Clear();
        }
    }
}