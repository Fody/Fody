using System;
using System.IO;
using System.Linq;
using Moq;
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
            var innerWeavingTask = new Processor();
            innerWeavingTask.ConfigFiles.Add(fileName);
            innerWeavingTask.CheckForWeaversXmlChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }
    [Test]
    public void AddExistingFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var innerWeavingTask = new Processor();
            innerWeavingTask.ConfigFiles.Add(fileName);
            innerWeavingTask.CheckForWeaversXmlChanged();
            innerWeavingTask.CheckForWeaversXmlChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }

    [Test]
    public void AddChangedFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var expected = File.GetLastWriteTimeUtc(fileName);
            var loggerMock = new Mock<BuildLogger>();
            loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

            var innerWeavingTask = new Processor
                                       {
                                     Logger = loggerMock.Object 
                                 };
            innerWeavingTask.ConfigFiles.Add(fileName);
            innerWeavingTask.CheckForWeaversXmlChanged();
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            innerWeavingTask.CheckForWeaversXmlChanged();

            loggerMock.Verify();

            Assert.AreEqual(expected, Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }
}