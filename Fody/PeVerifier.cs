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
    public static bool FoundPeVerify;
    public static string WindowsSdkDirectory;
    public static string PeverifyPath;

    static Verifier()
    {
        var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        WindowsSdkDirectory = Path.Combine(programFilesPath, @"Microsoft SDKs\Windows");
        if (!Directory.Exists(WindowsSdkDirectory))
        {
            FoundPeVerify = false;
            return;
        }
        PeverifyPath = Directory.EnumerateFiles(WindowsSdkDirectory, "peverify.exe", SearchOption.AllDirectories)
            .Where(x => !x.ToLowerInvariant().Contains("x64"))
            .OrderByDescending(x => FileVersionInfo.GetVersionInfo(x).FileVersion)
            .FirstOrDefault();
        if (PeverifyPath == null)
        {
            FoundPeVerify = false;
            return;
        }
        FoundPeVerify = true;
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
            Logger.LogInfo($"  Finished verification in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }


    bool InnerVerify()
    {
        List<string> ignoreCodes;
        if (!ReadShouldVerifyAssembly(out ignoreCodes))
        {
            Logger.LogInfo("  Skipped Verifying assembly since it is disabled in configuration");
            return true;
        }

        if (!FoundPeVerify)
        {
            Logger.LogInfo($"Skipped Verifying assembly since could not find peverify.exe in '{WindowsSdkDirectory}'.");
            return true;
        }

        if (!File.Exists(TargetPath))
        {
            Logger.LogInfo($"Cannot verify assembly, file '{TargetPath}' does not exist");
            return true;
        }

        Logger.LogInfo("  Verifying assembly");
        Logger.LogDebug($"Running verifier using command line {PeverifyPath} {TargetPath}");

        var processStartInfo = new ProcessStartInfo(PeverifyPath)
                               {
                                   Arguments = $"\"{TargetPath}\" /hresult /nologo /ignore=0x80070002,{string.Join(",", ignoreCodes)}",
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
                Logger.LogError($"PEVerify of the assembly failed.\n{output}");
                return false;
            }
        }
        return true;
    }

    
    public bool ReadShouldVerifyAssembly(out List<string> ignoreCodes)
    {
        var weaverConfigs = ConfigFileFinder.FindWeaverConfigs(SolutionDirectory, ProjectDirectory, Logger);
        ignoreCodes = ExtractVerifyIgnoreCodesConfigs(weaverConfigs).ToList();
        if (DefineConstants.Any(x => x == "FodyVerifyAssembly"))
        {
            return true;
        }


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

    public static IEnumerable<string> ExtractVerifyIgnoreCodesConfigs(List<string> weaverConfigs)
    {
        foreach (var configFile in weaverConfigs)
        {
            var configXml = XDocument.Load(configFile);
            var element = configXml.Root;
            var codesConfigs = (string) element.Attribute("VerifyIgnoreCodes");
            if (!string.IsNullOrWhiteSpace(codesConfigs))
            {
                foreach (var value in codesConfigs.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return value;
                }
            }
        }
    }
}