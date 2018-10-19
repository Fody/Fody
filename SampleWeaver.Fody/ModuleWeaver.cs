using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var intermediateFoder = Path.GetDirectoryName(ModuleDefinition.FileName);
        var additionalFilePath = Path.Combine(intermediateFoder, "SomeExtraFile.txt");

        File.WriteAllText(additionalFilePath, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        ReferenceCopyLocalPaths.Add(additionalFilePath);

        var customAttributes = ModuleDefinition.Assembly.CustomAttributes;

        var sampleAttr = customAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "SampleAttribute");
        if (sampleAttr != null)
        {
            customAttributes.Remove(sampleAttr);
            var filePath = sampleAttr.AttributeType.Resolve().Module.FileName;

            ReferenceCopyLocalPaths.Remove(filePath);
            ReferenceCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".pdb"));
            ReferenceCopyLocalPaths.Remove(Path.ChangeExtension(filePath, ".xml"));
        }

    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }

    public override bool ShouldCleanReference => true;
}