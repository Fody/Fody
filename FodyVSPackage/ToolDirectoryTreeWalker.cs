using System.Collections.Generic;
using System.IO;

public static class ToolDirectoryTreeWalker
{
    public static IEnumerable<string> TreeWalkForToolsDirs(string currentDirectory)
    {
        while (true)
        {
            var toolsDir = Path.Combine(currentDirectory, "Tools");
            if (Directory.Exists(toolsDir))
            {
                yield return toolsDir;
            }
            try
            {
                var parent = Directory.GetParent(currentDirectory);
                if (parent == null)
                {
                    yield break;
                }
                currentDirectory = parent.FullName;
            }
            catch
            {
                // trouble with tree walk. log and ignore
                yield break;
            }
        }
    }
}