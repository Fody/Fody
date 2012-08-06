using System.IO;

public static class FodyDirectoryFinder
{
    public static string TreeWalkForToolsFodyDir(string currentDirectory)
    {
        while (true)
        {
            var fodyDir = Path.Combine(currentDirectory, @"Tools\Fody");
            if (Directory.Exists(fodyDir))
            {
                return fodyDir;
            }
            try
            {
                var parent = Directory.GetParent(currentDirectory);
                if (parent == null)
                {
                    break;
                }
                currentDirectory = parent.FullName;
            }
            catch
            {
                // trouble with tree walk.
                return null;
            }
        }
        return null;
    }
}