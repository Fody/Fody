using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fody;

public partial class InnerWeaver
{
    static void ValidatePackageReferenceSettings(IList<WeaverHolder> weaverInstances, string projectFilePath, ILogger logger)
    {
        var weaversWithoutReference = weaverInstances.Where(weaver => weaver.Instance.ShouldCleanReference).ToList();

        if (!weaversWithoutReference.Any())
        {
            logger.LogDebug("No weaver with 'ShouldCleanReference=true' found, skip package reference validation");
            return;
        }

        var weaverNames = weaversWithoutReference
            .Select(weaver => Path.GetFileNameWithoutExtension(weaver.Config.AssemblyPath))
            .ToList();

        logger.LogDebug($"Weavers with 'ShouldCleanReference=true': {string.Join(", ", weaverNames)}");

        var projectFile = XDocumentEx.Load(projectFilePath);

        var errors = GetPackageReferenceValidationErrors(projectFile, weaverNames);

        foreach (var error in errors)
        {
            logger.LogWarning(error, "FodyPackageReference");
        }
    }

    public static IEnumerable<string> GetPackageReferenceValidationErrors(XDocument projectFile, IEnumerable<string> weaverNames)
    {
        var packageReferenceNodes = projectFile.Root
            .GetChildren("ItemGroup")
            .SelectMany(item => item.GetChildren("PackageReference"))
            .ToList();

        var errors = weaverNames
            .Select(weaverName => packageReferenceNodes.Where(reference => string.Equals(reference.Attribute("Include")?.Value, weaverName, StringComparison.OrdinalIgnoreCase)))
            .SelectMany(packageReferences => packageReferences.SelectMany(GetPackageReferenceValidationErrors));

        return errors.Distinct();
    }

    static IEnumerable<string> GetPackageReferenceValidationErrors(XElement packageReference)
    {
        var weaverName = packageReference.Attribute("Include").Value;

        if (!string.Equals(packageReference.GetAttributeOrNode("PrivateAssets"), "All", StringComparison.OrdinalIgnoreCase))
        {
            yield return $"The package reference for {weaverName} does not contain PrivateAssets='All'";
        }

        var includeAssets = packageReference.GetAttributeOrNode("IncludeAssets");
        if (includeAssets != null && !string.Equals(includeAssets, "all", StringComparison.OrdinalIgnoreCase) && !includeAssets.Split(';').Select(item => item.Trim()).Contains("compile", StringComparer.OrdinalIgnoreCase))
        {
            yield return $"The package reference for {weaverName} is missing the 'compile' part in the IncludeAssets setting; it's recommended to completely remove IncludeAssets";
        }
    }
}

public static partial class ExtensionMethods
{
    public static IEnumerable<XElement> GetChildren(this XElement element, string name)
    {
        return element.Descendants()
            .Where(item => item.Parent == element)
            .Where(item => item.Name.LocalName == name);
    }

    public static string? GetAttributeOrNode(this XElement element, string name)
    {
        return element.Attribute(name)?.Value.Trim() ?? element.GetChildren(name).FirstOrDefault()?.Value.Trim();
    }
}