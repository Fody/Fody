using System.IO;

class SolutionDirectoryFinder
{
    public static string Find(string solutionDir, string nCrunchOriginalSolutionDir, string projectDirectory)
    {
        if (nCrunchOriginalSolutionDir != null)
        {
            return nCrunchOriginalSolutionDir;
        }

        if (solutionDir != null)
        {
            return solutionDir;
        }

        return Directory.GetParent(projectDirectory).FullName;
    }
}