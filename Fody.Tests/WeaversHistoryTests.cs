using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class WeaversHistoryTests
{
    [Test]
    public void AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            Assert.IsFalse(hasChanged);
            Assert.AreEqual(fileName, WeaversHistory.TimeStamps.First().Key);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Test]
    public void Changed()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] {fileName});
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            Assert.IsTrue(hasChanged);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Test]
    public void Same()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] { fileName });

            var hasChanged = WeaversHistory.HasChanged(new[] { fileName });
            Assert.IsFalse(hasChanged);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }
}