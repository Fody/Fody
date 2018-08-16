using System.IO;

static class IoHelper
{
    public static void PurgeDirectory(string path)
    {
        var directoryInfo = new DirectoryInfo(path);

        foreach (var file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
        foreach (var dir in directoryInfo.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}