using System.Reflection;

public static class VersionChecker
{
    public static bool IsVersionNewer(string targetFile)
    {
        var existingVersion = AssemblyName.GetAssemblyName(targetFile).Version;
        return existingVersion < CurrentVersion.Version;
    }
}