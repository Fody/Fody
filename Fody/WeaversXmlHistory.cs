using System.Collections.Generic;
using System.Linq;

public static class WeaversConfigHistory
{
    static string WeaverConfigsContent;

    public static bool HasChanged(IEnumerable<WeaverConfigFile> configFiles)
    {
        var content = GetWeaverConfigsContent(configFiles);

        if (WeaverConfigsContent != null && !string.Equals(WeaverConfigsContent, content))
        {
            return true;
        }

        return false;
    }

    static string GetWeaverConfigsContent(IEnumerable<WeaverConfigFile> configFiles)
    {
        return string.Join("|", configFiles.Select(configFile => configFile.Document.ToString()));
    }

    public static void RegisterSnapshot(IEnumerable<WeaverConfigFile> configFiles)
    {
        WeaverConfigsContent = GetWeaverConfigsContent(configFiles);
    }
}