static class SdkToolFinder
{
    static string windowsSdkDirectory;

    static SdkToolFinder()
    {
        var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        windowsSdkDirectory = Path.Combine(programFilesPath, @"Microsoft SDKs\Windows");
        if (Directory.Exists(windowsSdkDirectory))
        {
            foundToolsDirectory = true;
        }
    }

    static bool foundToolsDirectory;

    public static bool TryFindTool(string tool, [NotNullWhen(true)] out string? path)
    {
        if (!foundToolsDirectory)
        {
            path = null;
            return false;
        }

        path = Directory.EnumerateFiles(windowsSdkDirectory, $"{tool}.exe", SearchOption.AllDirectories)
            .Where(x => !x.ToLowerInvariant().Contains("x64"))
            .OrderByDescending(x =>
            {
                var info = FileVersionInfo.GetVersionInfo(x);
                return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
            })
            .FirstOrDefault();
        return path != null;
    }
}