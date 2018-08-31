using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fody;
#pragma warning disable 618

public class Verifier
{
    public ILogger Logger;
    public string SolutionDirectory;
    public List<string> DefineConstants;
    public string ProjectDirectory;
    public string TargetPath;

    public bool Verify()
    {
        try
        {
            return InnerVerify();
        }
        catch (Exception exception)
        {
            Logger.LogException(exception);
            return false;
        }
    }

    bool InnerVerify()
    {
        if (!File.Exists(TargetPath))
        {
            Logger.LogInfo($"  Cannot verify assembly, file '{TargetPath}' does not exist");
            return true;
        }

        if (!ReadShouldVerifyAssembly(out var ignoreCodes))
        {
            Logger.LogInfo("  Skipped Verifying assembly since it is disabled in configuration");
            return true;
        }

        if (!PeVerifier.FoundPeVerify)
        {
            Logger.LogInfo("  Skipped Verifying assembly since could not find peverify.exe.");
            return true;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            Logger.LogInfo("  Verifying assembly");
            ignoreCodes.Add("0x80070002");
            ignoreCodes.Add("0x80131252");
            if (PeVerifier.Verify(TargetPath, ignoreCodes, out var output))
            {
                return true;
            }

            Logger.LogError($"PEVerify of the assembly failed.\n{output}");
            return false;
        }
        finally
        {

            Logger.LogInfo($"  Finished verification in {stopwatch.ElapsedMilliseconds}ms.");
        }
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
            if (element.TryReadBool("VerifyAssembly", out var value))
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
            if (string.IsNullOrWhiteSpace(codesConfigs))
            {
                continue;
            }
            foreach (var value in codesConfigs.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }
        }
    }
}