using System.IO;
using System.Linq;

// WeaversHistory uses static state
[NotInParallel]
public class WeaversHistoryTests
{
    [Test]
    public async Task AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            await Assert.That(hasChanged).IsFalse();
            await Assert.That(WeaversHistory.TimeStamps.First().Key).IsEqualTo(fileName);
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Test]
    public async Task Changed()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] {fileName});
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            var hasChanged = WeaversHistory.HasChanged(new[] {fileName});
            await Assert.That(hasChanged).IsTrue();
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }

    [Test]
    public async Task Same()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            WeaversHistory.HasChanged(new[] { fileName });

            var hasChanged = WeaversHistory.HasChanged(new[] { fileName });
            await Assert.That(hasChanged).IsFalse();
        }
        finally
        {
            File.Delete(fileName);
            WeaversHistory.TimeStamps.Clear();
        }
    }
}