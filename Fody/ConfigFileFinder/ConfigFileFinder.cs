using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public static class ConfigFileFinder
{
    const string FodyWeaversConfigFileName = "FodyWeavers.xml";
    static readonly XNamespace schemaNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
    static readonly XNamespace schemaInstanceNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

    public static IEnumerable<WeaverConfigFile> FindWeaverConfigFiles(string? weaverConfiguration, string solutionDirectoryPath, string projectDirectory, ILogger logger)
    {
        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, FodyWeaversConfigFileName);

        if (File.Exists(solutionConfigFilePath))
        {
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
            yield return new WeaverConfigFile(solutionConfigFilePath, true);
        }

        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);

        if (File.Exists(projectConfigFilePath))
        {
            logger.LogDebug($"Found path to weavers file '{projectConfigFilePath}'.");
            yield return new WeaverConfigFile(projectConfigFilePath);
        }

        if (!string.IsNullOrEmpty(weaverConfiguration))
        {
            logger.LogDebug("Found weaver configuration in project.");
            yield return new WeaverConfigFile(XDocumentEx.Parse(weaverConfiguration!));
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
                (
                    executionOrder: executionOrder,
                    configFile: configFile,
                    elementName: elementName,
                    content: element.ToString()
                );
            }
        }

        return entries;
    }

    public static WeaverConfigFile GenerateDefault(string projectDirectory, List<WeaverEntry> weaverEntries, bool generateXsd)
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

        var writerSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        };
        using (var writer = XmlWriter.Create(projectConfigFilePath, writerSettings))
        {
            weaverConfig.Save(writer);
        }

        if (generateXsd)
        {
            CreateSchemaForConfig(projectConfigFilePath, weaverEntries);
        }

        return new WeaverConfigFile(projectConfigFilePath);
    }

    static void CreateSchemaForConfig(string projectConfigFilePath, IEnumerable<WeaverEntry> weavers)
    {
        var schema = XDocument.Parse(Fody.Properties.Resources.FodyWeavers_SchemaTemplate);

        var baseNode = schema.Descendants().First(item => item.Name == schemaNamespace.GetName("all"));

        var fragments = weavers.Select(CreateItemFragment);

        baseNode.Add(fragments);

        var filePath = Path.ChangeExtension(projectConfigFilePath, ".xsd");

        const SaveOptions saveOptions = SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting;

        if (File.Exists(filePath))
        {
            try
            {
                var existing = XDocumentEx.Load(filePath).ToString(saveOptions);
                if (string.Equals(existing, schema.ToString(saveOptions)))
                {
                    // don't touch existing file if it is up to date
                    return;
                }
            }
            catch
            {
                // invalid xsd, overwrite always...
            }
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

        var fragmentFile = Path.ChangeExtension(weaverFile, ".xcf");

        if (File.Exists(fragmentFile))
        {
            try
            {
                element.Add(XElement.Parse(File.ReadAllText(fragmentFile)));
                return element;
            }
            catch (Exception exception)
            {
                throw new WeavingException($"The fragment file ({fragmentFile}) could not be read. Exception message: {exception.Message}");
            }
        }

        element.Add(new XAttribute("type", "xs:anyType"));

        return element;
    }

    public static void EnsureSchemaIsUpToDate(string projectDirectory, IEnumerable<WeaverEntry> weavers, bool defaultGenerateXsd)
    {
        var projectConfigFilePath = Path.Combine(projectDirectory, FodyWeaversConfigFileName);
        try
        {
            if (!File.Exists(projectConfigFilePath))
                return;

            var doc = XDocumentEx.Load(projectConfigFilePath);
            if (!ShouldGenerateXsd(doc, defaultGenerateXsd))
            {
                return;
            }

            var hasNamespace = doc.Root.Attributes()
                .Any(attr => !attr.IsNamespaceDeclaration &&
                             attr.Name.LocalName == "noNamespaceSchemaLocation" &&
                             string.Equals(attr.Value, "FodyWeavers.xsd", StringComparison.OrdinalIgnoreCase));

            if (!hasNamespace)
            {
                doc.Root.Add(SchemaInstanceAttributes);
                doc.Save(projectConfigFilePath);
            }

            CreateSchemaForConfig(projectConfigFilePath, weavers);
        }
        catch (Exception exception)
        {
            throw new WeavingException($"Failed to update schema for ({projectConfigFilePath}). Exception message: {exception.Message}");
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