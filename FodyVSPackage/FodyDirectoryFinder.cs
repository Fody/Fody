using System.IO;
using System.Linq;

public static class FodyDirectoryFinder
{
    public static string TreeWalkForToolsFodyDir(string solutionDirectory)
    {
        return ToolDirectoryTreeWalker.TreeWalkForToolsDirs(solutionDirectory)
            .Select(toolsDir => Path.Combine(toolsDir, @"Tools\Fody"))
            .FirstOrDefault(Directory.Exists);
    }
}