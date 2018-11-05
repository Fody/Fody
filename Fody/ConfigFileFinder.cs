using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Fody;

public static class ConfigFile
{
    private static readonly XNamespace schemaNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
    private static readonly XNamespace schemaInstanceNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

    public static List<string> FindWeaverConfigs(string solutionDirectoryPath, string projectDirectory, ILogger logger, IEnumerable<string> wellKnownWeaverFiles)
    {
        var files = new List<string>();

        var solutionConfigFilePath = Path.Combine(solutionDirectoryPath, "FodyWeavers.xml");
        if (File.Exists(solutionConfigFilePath))
        {
            files.Add(solutionConfigFilePath);
            logger.LogDebug($"Found path to weavers file '{solutionConfigFilePath}'.");
        }

        var projectConfigFilePath = Path.Combine(projectDirectory, "FodyWeavers.xml");
        if (!File.Exists(projectConfigFilePath))
        {
            if (!files.Any() && wellKnownWeaverFiles != null)
            {
                GenerateDefault(projectConfigFilePath, wellKnownWeaverFiles);

                logger.LogError($@"Could not file a FodyWeavers.xml at the project level ({projectConfigFilePath}). A default file has been created. Please review the file and add it to your project.");
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

            EnsureSchemaIsUpToDate(projectConfigFilePath, wellKnownWeaverFiles);
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
            return;

        var weaverConfig = new XDocument(new XElement("Weavers", SchemaInstanceAttributes));

        var weaverEntries = wellKnownWeaverFiles
            .Select(WeaverNameFromFilePath)
            .Select(name => new XElement(name))
            .ToArray();

        weaverConfig.Root.Add(weaverEntries);
        weaverConfig.Save(projectConfigFilePath);

        CreateSchemaForConfig(projectConfigFilePath, wellKnownWeaverFiles);
    }

    static void CreateSchemaForConfig(string projectConfigFilePath, IEnumerable<string> wellKnownWeaverFiles)
    {
        if (wellKnownWeaverFiles == null)
            return;

        var schema = XDocument.Parse(Fody.Properties.Resources.FodyWeavers_SchemaTemplate);

        var baseNode = schema.Descendants().FirstOrDefault(item => item.Name == schemaNamespace.GetName("all"));

        var fragments = wellKnownWeaverFiles.Select(CreateItemFragment);

        baseNode.Add(fragments);

        var filePath = Path.ChangeExtension(projectConfigFilePath, ".xsd");

        try
        {
            if (File.Exists(filePath) && string.Equals(XDocument.Load(filePath).ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting), schema.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting)))
            {
                // don't touch existing file if it is up to date
                return;
            }
        }
        catch
        {
            // invalid xsd, overwrite always...
        }

        schema.Save(filePath, SaveOptions.OmitDuplicateNamespaces);
    }

    private static XElement CreateItemFragment(string weaverFile)
    {
        var weaverName = WeaverNameFromFilePath(weaverFile);

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

    private static string WeaverNameFromFilePath(string filePath)
    {
        return Path.ChangeExtension(Path.GetFileNameWithoutExtension(filePath), null);
    }

    static void EnsureSchemaIsUpToDate(string projectConfigFilePath, IEnumerable<string> wellKnownWeaverFiles)
    {
        if (wellKnownWeaverFiles == null)
            return;

        try
        {
            var doc = XDocument.Load(projectConfigFilePath);

            var hasNamespace = doc.Root.Attributes()
                .Any(attr => !attr.IsNamespaceDeclaration && attr.Name.LocalName == "noNamespaceSchemaLocation" && string.Equals(attr.Value, "FodyWeavers.xsd", StringComparison.OrdinalIgnoreCase));

            if (!hasNamespace)
            {
                doc.Root.Add(SchemaInstanceAttributes);
                doc.Save(projectConfigFilePath);
            }

            CreateSchemaForConfig(projectConfigFilePath, wellKnownWeaverFiles);
        }
        catch
        {
            // anything wrong with the existing, ignore here, we will warn later... 
        }
    }

    static XAttribute[] SchemaInstanceAttributes =>

        new []
    {
        new XAttribute(XNamespace.Xmlns + "xsi", schemaInstanceNamespace.NamespaceName),
        new XAttribute(schemaInstanceNamespace.GetName("noNamespaceSchemaLocation"), "FodyWeavers.xsd"),
    };
}
