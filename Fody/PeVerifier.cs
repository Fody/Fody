using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class Verifier
{
    public ILogger Logger;
    public string SolutionDirectory;
    public List<string> DefineConstants;
    public string ProjectDirectory;
    public string TargetPath;
    public static bool foundPeVerify;
    public static string windowsSdkDirectory;
    public static string peverifyPath;

    static Verifier()
    {
        var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        windowsSdkDirectory = Path.Combine(programFilesPath, @"Microsoft SDKs\Windows");
        if (!Directory.Exists(windowsSdkDirectory))
        {
            foundPeVerify = false;
            return;
        }
        peverifyPath = Directory.EnumerateFiles(windowsSdkDirectory, "peverify.exe", SearchOption.AllDirectories)
            .Where(x => !x.ToLowerInvariant().Contains("x64"))
            .OrderByDescending(x => FileVersionInfo.GetVersionInfo(x).FileVersion)
            .FirstOrDefault();
        if (peverifyPath == null)
        {
            foundPeVerify = false;
            return;
        }
        foundPeVerify = true;
    }

    public bool Verify()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return InnerVerify();
        }
        catch (Exception exception)
        {
            Logger.LogException(exception);
            return false;
        }
        finally
        {
            Logger.LogInfo(string.Format("  Finished verification in {0}ms.", stopwatch.ElapsedMilliseconds));
        }
    }


    bool InnerVerify()
    {
        if (!ReadShouldVerifyAssembly())
        {
            Logger.LogInfo("  Skipped Verifying assembly since it is disabled in configuration");
            return true;
        }

        if (!foundPeVerify)
        {
            Logger.LogInfo(string.Format("Skipped Verifying assembly since could not find peverify.exe in '{0}'.", windowsSdkDirectory));
            return true;
        }

        if (!File.Exists(TargetPath))
        {
            Logger.LogInfo(string.Format("Cannot verify assembly, file '{0}' does not exist", TargetPath));
            return true;
        }

        Logger.LogInfo("  Verifying assembly");
        Logger.LogDebug(string.Format("Running verifier using command line {0} {1}", peverifyPath, TargetPath));

        var processStartInfo = new ProcessStartInfo(peverifyPath)
                               {
                                   Arguments = string.Format("\"{0}\" /ignore=0x80070002", TargetPath),
                                   WorkingDirectory = Path.GetDirectoryName(TargetPath),
                                   CreateNoWindow = true,
                                   UseShellExecute = false,
                                   RedirectStandardOutput = true
                               };

        using (var process = Process.Start(processStartInfo))
        {
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Logger.LogError(string.Format("PEVerify of the assembly failed.\n{0}", output));
                return false;
            }
        }
        return true;
    }

    
    public bool ReadShouldVerifyAssembly()
    {
        if (DefineConstants.Any(x => x == "FodyVerifyAssembly"))
        {
            return true;
        }

        var weaverConfigs = ConfigFileFinder.FindWeaverConfigs(SolutionDirectory, ProjectDirectory, Logger);

        return ExtractVerifyAssemblyFromConfigs(weaverConfigs);
    }

    public static bool ExtractVerifyAssemblyFromConfigs(List<string> weaverConfigs)
    {
        foreach (var configFile in weaverConfigs)
        {
            var configXml = XDocument.Load(configFile);
            var element = configXml.Root;
            bool value;
            if (element.TryReadBool("VerifyAssembly", out value))
            {
                return value;
            }
        }
        return false;
    }
}