using Fody;

using System;
using System.Linq;

public partial class Processor
{
    public void ConfigureWhenWeaversFound()
    {
        foreach (var weaverConfig in Weavers)
        {
            ProcessConfig(weaverConfig);
        }
    }

    public void ProcessConfig(WeaverEntry weaverConfig)
    {
        //support for diff names sources
        if (weaverConfig.Source != null)
        {
            weaverConfig.AssemblyPath = References.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(reference => reference.EndsWith($"{weaverConfig.Source}.dll", StringComparison.InvariantCultureIgnoreCase)) ?? throw new WeavingException($"Assembly {weaverConfig.Source} could not be found.");
            weaverConfig.TypeName = weaverConfig.AssemblyName;
            WeaverProjectUsed = true;
            return;
        }

        //support for diff names weavers when "In solution weaving"        
        var weaverProjectContains = WeaverProjectContainsType(weaverConfig.AssemblyName);
        if (weaverProjectContains)
        {
            weaverConfig.AssemblyPath = WeaverAssemblyPath;
            weaverConfig.TypeName = weaverConfig.AssemblyName;
            return;
        }

        var assemblyPath = FindAssemblyPath(weaverConfig.AssemblyName);
        if (assemblyPath == null)
        {
            var message = string.Format(
                @"Could not find a weaver named '{0}'.
If you have nuget package restore turned on you probably need to do a build to download the weavers.
Alternatively you may have added a weaver to your 'FodyWeavers.xml' and forgot to add the appropriate NuGet package.
Perhaps you need to run 'Install-Package {0}.Fody'. This url may provide more information http://nuget.org/packages/{0}.Fody/ .", weaverConfig.AssemblyName);
            throw new WeavingException(message);
        }
        weaverConfig.AssemblyPath = assemblyPath;
        weaverConfig.TypeName = "ModuleWeaver";
    }
}