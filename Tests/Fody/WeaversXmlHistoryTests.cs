using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Xunit;

public class WeaversXmlHistoryTests : TestBase
{
    [Fact]
    public void AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var processor = new Processor
            {
                ConfigFiles = new List<string>
                                              {
                                                  fileName
                                              }
            };
            processor.CheckForWeaversXmlChanged();

            Assert.Equal(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }

    [Fact]
    public void AddExistingFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var processor = new Processor
                            {
                                ConfigFiles = new List<string>
                                              {
                                                  fileName
                                              }
                            };
            processor.CheckForWeaversXmlChanged();
            processor.CheckForWeaversXmlChanged();

            Assert.Equal(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }

    [Fact]
    public void AddChangedFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var expected = File.GetLastWriteTimeUtc(fileName);
            var loggerMock = new Mock<BuildLogger>();
            loggerMock.Setup(x => x.LogDebug(It.IsAny<string>()));

            var processor = new Processor
                            {
                                Logger = loggerMock.Object,
                                ConfigFiles = new List<string>
                                              {
                                                  fileName
                                              }
                            };
            processor.CheckForWeaversXmlChanged();
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            processor.CheckForWeaversXmlChanged();

            loggerMock.Verify();

            Assert.Equal(expected, Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }
}