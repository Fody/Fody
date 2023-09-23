using System.IO;

public class SolutionDirectoryFinder
{
    public static string Find(string? solutionDir, string? nCrunchOriginalSolutionDir, string projectDirectory)
    {
        if (!string.IsNullOrEmpty(nCrunchOriginalSolutionDir))
        {
            return nCrunchOriginalSolutionDir!;
        }

        if (!string.IsNullOrEmpty(solutionDir) && solutionDir != "*Undefined*")
        {
            return solutionDir!;
        }

        return Directory.GetParent(projectDirectory).FullName;
    }
}