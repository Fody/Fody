using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class TaskFileReplacer
{
    MessageDisplayer messageDisplayer;
    public string TaskFilePath;
    ContentsFinder contentsFinder;

    public TaskFileReplacer(MessageDisplayer messageDisplayer, ContentsFinder contentsFinder)
    {
        this.messageDisplayer = messageDisplayer;
        this.contentsFinder = contentsFinder;
        var appData = Environment.GetEnvironmentVariable("appdata");
        Directory.CreateDirectory(Path.Combine(appData,"Fody"));
        TaskFilePath = Path.Combine(appData,"Fody","TaskAssembliesToUpdate.txt");
        if (!File.Exists(TaskFilePath))
        {
            using (File.Create(TaskFilePath))
            {
            }
        }
    }

    public void CheckForFilesToUpdate()
    {
        ThreadPool.QueueUserWorkItem(x =>
                                         {
                                             bool createdNew;
                                             using (new Mutex(true, typeof (TaskFileReplacer).FullName, out createdNew))
                                             {
                                                 if (!createdNew)
                                                 {
                                                     //already being used;
                                                     return;
                                                 }
                                                 var newStrings = new List<string>();
                                                 foreach (var targetDirectory in File.ReadAllLines(TaskFilePath))
                                                 {
                                                     var trimmed = targetDirectory.Trim();
                                                     if (trimmed.Length == 0)
                                                     {
                                                         continue;
                                                     }
                                                     var directoryInfo = new DirectoryInfo(trimmed);
                                                     if (!directoryInfo.Exists)
                                                     {
                                                         continue;
                                                     }
                                                     DeployFiles(newStrings, trimmed);
                                                 }
                                                 File.WriteAllLines(TaskFilePath, newStrings);
                                             }
                                         });
    }

    void DeployFiles(List<string> newStrings, string trimmed)
    {
        var success = false;
        try
        {
            foreach (var file in Directory.GetFiles(Path.Combine(contentsFinder.ContentFilesPath, "Fody")))
            {
                File.Copy(file, Path.Combine(trimmed, Path.GetFileName(file)), true);
                success = true;
            }
            var info = string.Format("Fody: Updated '{0}' to version {1}.", trimmed, CurrentVersion.Version);
            messageDisplayer.ShowInfo(info);
        }
        catch (Exception exception)
        {
            if (success)
            {
                var error = string.Format("Fody: Failed to update '{0}' to version {1} due to '{2}'. Please manually copy the contents of '{3}' to '{0}'.", trimmed, CurrentVersion.Version, exception.Message, contentsFinder.ContentFilesPath);
                messageDisplayer.ShowError(error);
            }
            else
            {
                var info = string.Format("Fody: Failed to update '{0}' to version {1} will try again later.", trimmed, CurrentVersion.Version);
                messageDisplayer.ShowInfo(info);
                newStrings.Add(trimmed);
            }
        }
    }

    public void AddFile(string directoryInfo)
    {
        using (var mutex = new Mutex(true, typeof (TaskFileReplacer).FullName))
        {
            if (!mutex.WaitOne(100))
            {
                return;
            }
            var allText = File.ReadAllLines(TaskFilePath);
            var fileContainsDirectory = allText.Any(line => string.Equals(line, directoryInfo, StringComparison.InvariantCultureIgnoreCase));
            if (!fileContainsDirectory)
            {
                messageDisplayer.ShowInfo("Fody: Restart of Visual Studio required to update Fody.");
                File.AppendAllText(TaskFilePath, directoryInfo + "\r\n");
            }
        }
    }

}