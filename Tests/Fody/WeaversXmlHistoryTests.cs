using System.IO;
using Xunit;

public class WeaversXmlHistoryTests : TestBase
{
    [Fact]
    public void Test()
    {
        var fileName = Path.GetTempFileName();

        try
        {
            File.WriteAllText(fileName, "<Weavers />");

            var configFiles1 = new[]
            {
                new WeaverConfigFile(fileName)
            };

            var hasChanged = WeaversConfigHistory.HasChanged(configFiles1);

            Assert.False(hasChanged);

            WeaversConfigHistory.RegisterSnapshot(configFiles1);

            var configFiles2 = new[]
            {
                new WeaverConfigFile(fileName)
            };

            hasChanged =  WeaversConfigHistory.HasChanged(configFiles2);

            Assert.False(hasChanged);

            WeaversConfigHistory.RegisterSnapshot(configFiles2);

            File.WriteAllText(fileName, "<Weavers VerifyAssembly='true' />");

            var configFiles3 = new[]
            {
                new WeaverConfigFile(fileName)
            };

            hasChanged =  WeaversConfigHistory.HasChanged(configFiles3);

            Assert.True(hasChanged);
        }
        finally
        {
            File.Delete(fileName);
        }
    }
}
