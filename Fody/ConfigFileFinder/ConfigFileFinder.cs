using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public static class ConfigFileFinder
{
    const string FodyWeaversConfigFileName = "FodyWeavers.xml";
    static readonly XNamespace schemaNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
    static readonly XNamespace schemaInstanceNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

    public static IEnumerable<WeaverConfigFile> FindWeaverConfigFiles(string solutionDirectoryPath, string projectDirectory, ILogger logger)
    {
        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, FodyWeaversConfigFileName);

        if (File.Exists(solutionConfigFilePath))
        {
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
            yield return new WeaverConfigFile(solutionConfigFilePath) {IsGlobal = true};
        }

        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);

        if (File.Exists(projectConfigFilePath))
        {
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
            yield return new WeaverConfigFile(projectConfigFilePath);
        }
    }

    public static Dictionary<string, WeaverConfigEntry> ParseWeaverConfigEntries(IEnumerable<WeaverConfigFile> configFiles)
    {
        var entries = new Dictionary<string, WeaverConfigEntry>(StringComparer.OrdinalIgnoreCase);
        var executionOrders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var configFile in configFiles)
        {
            foreach (var element in configFile.Document.Root.Elements())
            {
                var elementName = element.Name.LocalName;

                if (!executionOrders.TryGetValue(elementName, out var executionOrder))
                {
                    executionOrders.Add(elementName, executionOrder = executionOrders.Count);
                }

                entries[elementName] = new WeaverConfigEntry
                {
                    ExecutionOrder = executionOrder,
                    ConfigFile = configFile,
                    ElementName = elementName,
                    Content = element.ToString()
                };
            }
        }

        return entries;
    }

    public static WeaverConfigFile GenerateDefault(string projectDirectory, IEnumerable<WeaverEntry> weaverEntries, bool generateXsd)
    {
        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);

        if (File.Exists(projectConfigFilePath))
        {
            return new WeaverConfigFile(projectConfigFilePath);
        }

        var root = new XElement("Weavers", SchemaInstanceAttributes);
        var weaverConfig = new XDocument(root);

        var elements = weaverEntries
            .Select(entry => new XElement(entry.ElementName))
            .ToArray();

        root.Add(elements);
        weaverConfig.Save(projectConfigFilePath);

        if (generateXsd)
        {
            CreateSchemaForConfig(projectConfigFilePath, weaverEntries);
        }

        return new WeaverConfigFile(projectConfigFilePath);
    }

    static void CreateSchemaForConfig(string projectConfigFilePath, IEnumerable<WeaverEntry> weavers)
    {
        var schema = XDocument.Parse(Fody.Properties.Resources.FodyWeavers_SchemaTemplate);

        var baseNode = schema.Descendants().FirstOrDefault(item => item.Name == schemaNamespace.GetName("all"));

        var fragments = weavers.Select(CreateItemFragment);

        baseNode.Add(fragments);

        var filePath = Path.ChangeExtension(projectConfigFilePath, ".xsd");

        try
        {
            if (File.Exists(filePath))
            {
                const SaveOptions saveOptions = SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting;
                if (string.Equals(XDocumentEx.Load(filePath).ToString(saveOptions), schema.ToString(saveOptions)))
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

    static XElement CreateItemFragment(WeaverEntry weaver)
    {
        var elementName = weaver.ElementName;
        var weaverFile = weaver.AssemblyPath;

        var element = new XElement(schemaNamespace.GetName("element"),
            new XAttribute("name", elementName),
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

    public static void EnsureSchemaIsUpToDate(string projectDirectory, IEnumerable<WeaverEntry> weavers, bool defaultGenerateXsd)
    {
        try
        {
            var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);

            var doc = XDocumentEx.Load(projectConfigFilePath);

            if (!ShouldGenerateXsd(doc, defaultGenerateXsd))
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

            CreateSchemaForConfig(projectConfigFilePath, weavers);
        }
        catch
        {
            //TODO: anything wrong with the existing, ignore here, we will warn later...
        }
    }

    static bool ShouldGenerateXsd(XDocument doc, bool defaultGenerateXsd)
    {
        if (doc.Root.TryReadBool("GenerateXsd", out var generateXsd))
        {
            return generateXsd;
        }

        return defaultGenerateXsd;
    }

    static XAttribute[] SchemaInstanceAttributes =>
        new[]
        {
            new XAttribute(XNamespace.Xmlns + "xsi", schemaInstanceNamespace.NamespaceName),
            new XAttribute(schemaInstanceNamespace.GetName("noNamespaceSchemaLocation"), "FodyWeavers.xsd"),
        };
}
