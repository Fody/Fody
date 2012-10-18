using System.IO;

public partial class AddinFinder
{
    public void AddToolsAssemblyLocationToAddinSearch()
    {
        var directoryInfo = new DirectoryInfo(AssemblyLocation.CurrentDirectory()).Parent;
        if (directoryInfo == null)
        {
            return;
        }
        directoryInfo = directoryInfo.Parent;
        if (directoryInfo == null)
        {
            return;
        }
        AddinSearchPaths.Add(directoryInfo.FullName);
    }
}