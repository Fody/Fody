using System.IO;
using System.Linq;

public class WeaversHistoryTests
{
    [Fact]
    public void AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            Assert.False(hasChanged);
            Assert.Equal(fileName, WeaversHistory.TimeStamps.First().Key);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Fact]
    public void Changed()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] {fileName});
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            Assert.True(hasChanged);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Fact]
    public void Same()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] { fileName });

            var hasChanged = WeaversHistory.HasChanged(new[] { fileName });
            Assert.False(hasChanged);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }
}