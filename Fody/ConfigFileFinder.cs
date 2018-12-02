using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Fody;

public static class ConfigFile
{
    private const string FodyWeaversConfigFileName = "FodyWeavers.xml";
    static readonly XNamespace schemaNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
    static readonly XNamespace schemaInstanceNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

    public static List<string> FindWeaverConfigs(string solutionDirectoryPath, string projectDirectory, ILogger logger, IEnumerable<string> wellKnownWeaverFiles)
    {
        var files = new List<string>();

        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, FodyWeaversConfigFileName);
        if (File.Exists(solutionConfigFilePath))
        {
            files.Add(solutionConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
        }

        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);
        if (!File.Exists(projectConfigFilePath))
        {
            if (!files.Any() && wellKnownWeaverFiles != null)
            {
                GenerateDefault(projectConfigFilePath, wellKnownWeaverFiles);

                logger.LogError($"Could not find a FodyWeavers.xml file at the project level ({projectConfigFilePath}). A default file has been created. Please review the file and add it to your project.");
            }
            else
            {
                logger.LogError($@"Could not find a FodyWeavers.xml file at the project level ({projectConfigFilePath}). Some project types do not support using NuGet to add content files e.g. netstandard projects. In these cases it is necessary to manually add a FodyWeavers.xml to the project. Example content:
  <Weavers>
    <WeaverName/>
  </Weavers>
  ");
            }
        }
        else
        {
            files.Add(projectConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
        }

        if (files.Count == 0)
        {
            var pathsSearched = string.Join("', '", solutionConfigFilePath, projectConfigFilePath);
            throw new WeavingException($"Could not find path to weavers file. Searched '{pathsSearched}'.");
        }

        return files;
    }

    static void GenerateDefault(string projectConfigFilePath, IEnumerable<string> wellKnownWeaverFiles)
    {
        if (wellKnownWeaverFiles == null)
        {
            return;
        }

        var weaverConfig = new XDocument(new XElement("Weavers", SchemaInstanceAttributes));

        var weaverEntries = wellKnownWeaverFiles
            .Select(WeaverNameFromFilePath)
            .Select(name => new XElement(name))
            .ToArray();

        weaverConfig.Root.Add(weaverEntries);
        weaverConfig.Save(projectConfigFilePath);

        CreateSchemaForConfig(projectConfigFilePath, wellKnownWeaverFiles, null);
    }

    static string GetWeaverName(WeaverEntry weaver)
    {
        return string.IsNullOrEmpty(weaver.Element) ? weaver.TypeName : weaver.AssemblyName;
    }

    static void CreateSchemaForConfig(string projectConfigFilePath, IEnumerable<string> wellKnownWeaverFiles, IEnumerable<WeaverEntry> weavers)
    {
        var weaverFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        weaverFiles.MergeItemsFrom(weavers, GetWeaverName, weaver => weaver.AssemblyPath);
        weaverFiles.MergeItemsFrom(wellKnownWeaverFiles, WeaverNameFromFilePath);

        var schema = XDocument.Parse(Fody.Properties.Resources.FodyWeavers_SchemaTemplate);

        var baseNode = schema.Descendants().FirstOrDefault(item => item.Name == schemaNamespace.GetName("all"));

        var fragments = weaverFiles.Select(CreateItemFragment);

        baseNode.Add(fragments);

        var filePath = Path.ChangeExtension(projectConfigFilePath, ".xsd");

        try
        {
            if (File.Exists(filePath))
            {
                if (string.Equals(XDocumentEx.Load(filePath).ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting), schema.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting)))
                {
                    // don't touch existing file if it is up to date
                    return;
                }
            }
        }
        catch
        {
            // invalid xsd, overwrite always...
        }

        schema.Save(filePath, SaveOptions.OmitDuplicateNamespaces);
    }

    static XElement CreateItemFragment(KeyValuePair<string, string> weaver)
    {
        var weaverName = weaver.Key;
        var weaverFile = weaver.Value;

        var element = new XElement(schemaNamespace.GetName("element"),
            new XAttribute("name", weaverName),
            new XAttribute("minOccurs", 0),
            new XAttribute("maxOccurs", 1));

        var fragmentFileName = Path.ChangeExtension(weaverFile, ".xcf");

        if (File.Exists(fragmentFileName))
        {
            try
            {
                element.Add(XElement.Parse(File.ReadAllText(fragmentFileName)));
                return element;
            }
            catch
            {
                // invalid fragment, ignore...
            }
        }

        element.Add(new XAttribute("type", "xs:anyType"));

        return element;
    }

    static string WeaverNameFromFilePath(string filePath)
    {
        return Path.ChangeExtension(Path.GetFileNameWithoutExtension(filePath), null);
    }

    static void MergeItemsFrom<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<TValue> items, Func<TValue, TKey> keySelector)
    {
        MergeItemsFrom(target, items, keySelector, value => value);
    }

    static void MergeItemsFrom<TKey, TValue, TItem>(this IDictionary<TKey, TValue> target, IEnumerable<TItem> items, Func<TItem, TKey> keySelector, Func<TItem, TValue> valueSelector)
    {
        if (items == null)
            return;

        foreach (var item in items)
        {
            target[keySelector(item)] = valueSelector(item);
        }
    }

    public static void EnsureSchemaIsUpToDate(string projectDirectory, IEnumerable<string> wellKnownWeaverFiles, List<WeaverEntry> weavers)
    {
        if (wellKnownWeaverFiles == null && weavers == null)
        {
            return;
        }

        try
        {
            var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);

            var doc = XDocumentEx.Load(projectConfigFilePath);

            if (doc.Root.TryReadBool("GenerateXsd", out var generateXsd) && !generateXsd)
            {
                return;
            }

            var hasNamespace = doc.Root.Attributes()
                .Any(attr => !attr.IsNamespaceDeclaration && attr.Name.LocalName == "noNamespaceSchemaLocation" && string.Equals(attr.Value, "FodyWeavers.xsd", StringComparison.OrdinalIgnoreCase));

            if (!hasNamespace)
            {
                doc.Root.Add(SchemaInstanceAttributes);
                doc.Save(projectConfigFilePath);
            }

            CreateSchemaForConfig(projectConfigFilePath, wellKnownWeaverFiles, weavers);
        }
        catch
        {
            // anything wrong with the existing, ignore here, we will warn later...
        }
    }

    static XAttribute[] SchemaInstanceAttributes =>

        new[]
    {
        new XAttribute(XNamespace.Xmlns + "xsi", schemaInstanceNamespace.NamespaceName),
        new XAttribute(schemaInstanceNamespace.GetName("noNamespaceSchemaLocation"), "FodyWeavers.xsd"),
    };
}
