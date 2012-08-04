using System;
using System.IO;

public class TaskFileProcessor
{
    TaskFileReplacer taskFileReplacer;
    MessageDisplayer messageDisplayer;

    public TaskFileProcessor(TaskFileReplacer taskFileReplacer, MessageDisplayer messageDisplayer)
    {
        this.taskFileReplacer = taskFileReplacer;
        this.messageDisplayer = messageDisplayer;
    }

    public void ProcessTaskFile(string solutionDirectory)
    {
        try
        {
            var fodyDir = FodyDirectoryFinder.TreeWalkForToolsFodyDir(solutionDirectory);
            if (fodyDir != null)
            {
                var fodyFile = Path.Combine(fodyDir, "Fody.dll");
                if (File.Exists(fodyFile))
                {
                    Check(fodyFile);
                    return;
                }
            }
            foreach (var filePath in Directory.EnumerateFiles(solutionDirectory, "Fody.dll", SearchOption.AllDirectories))
            {
                Check(filePath);
            }
        }
        catch (Exception exception)
        {
            messageDisplayer.ShowError(string.Format("Fody: An exception occured while trying to check for updates.\r\nException: {0}.", exception));
        }
    }

    void Check(string fodyFile)
    {
        if (VersionChecker.IsVersionNewer(fodyFile))
        {
            taskFileReplacer.AddFile(Path.GetDirectoryName(fodyFile));
        }
    }
}