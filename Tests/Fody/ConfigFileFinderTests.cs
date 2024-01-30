using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ConfigFileFinderTests :
    IDisposable
{
    static XNamespace schemaNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
    static XNamespace schemaInstanceNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

    string testDir;
    string slnDir;

    string xmlPath;
    string xsdPath;

    string slnXmlPath;
    string slnXsdPath;

    public ConfigFileFinderTests()
    {
        testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
        slnDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
        Directory.CreateDirectory(testDir);
        Directory.CreateDirectory(slnDir);

        xmlPath = Path.Combine(testDir, "FodyWeavers.xml");
        xsdPath = Path.Combine(testDir, "FodyWeavers.xsd");
        slnXmlPath = Path.Combine(slnDir, "FodyWeavers.xml");
        slnXsdPath = Path.Combine(slnDir, "FodyWeavers.xsd");
    }

    public void Dispose()
    {
        Directory.Delete(testDir, true);
        Directory.Delete(slnDir, true);
    }

    [Fact]
    public void ShouldCreateXsd()
    {
        File.WriteAllText(
            xmlPath,
            """
            <Weavers>
              <TestWeaver />
            </Weavers>
            """);

        File.WriteAllText(
            Path.Combine(testDir, "WeaverWithSchema.Fody.xcf"),
            """
            <xs:complexType xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:attribute name="TestAttribute" type="xs:string" />
            </xs:complexType>
            """);

        var weavers = new[]
        {
            new WeaverEntry
            {
                AssemblyPath = @"something\TestWeaver.Fody.dll"
            },
            new WeaverEntry
            {
                AssemblyPath = Path.Combine(testDir, "WeaverWithSchema.Fody.dll")
            }
        };

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, slnDir, testDir, new MockBuildLogger()).ToArray();

        ConfigFileFinder.EnsureSchemaIsUpToDate(testDir, weavers, true);

        Assert.Single(configFiles);
        Assert.False(configFiles[0].AllowExtraEntries);
        Assert.Equal(xmlPath, configFiles[0].FilePath);

        Assert.True(File.Exists(xsdPath));

        var xml = XDocumentEx.Load(xmlPath);
        Assert.NotNull(xml.Root);
        Assert.Equal("FodyWeavers.xsd", xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")?.Value);

        var xsd = XDocumentEx.Load(xsdPath);
        Assert.NotNull(xsd.Root);
        var elements = xsd.Root.Descendants(schemaNamespace + "all").First().Elements().ToList();

        Assert.Equal(2, elements.Count);

        var defaultElem = elements[0];
        Assert.Equal("element", defaultElem.Name.LocalName);
        Assert.Equal("TestWeaver", defaultElem.Attribute("name")?.Value);
        Assert.Equal("xs:anyType", defaultElem.Attribute("type")?.Value);
        Assert.Equal("0", defaultElem.Attribute("minOccurs")?.Value);
        Assert.Equal("1", defaultElem.Attribute("maxOccurs")?.Value);

        var elemWithSchema = elements[1];
        Assert.Equal("element", elemWithSchema.Name.LocalName);
        Assert.Equal("WeaverWithSchema", elemWithSchema.Attribute("name")?.Value);
        Assert.Null(elemWithSchema.Attribute("type"));
        Assert.Equal("0", elemWithSchema.Attribute("minOccurs")?.Value);
        Assert.Equal("1", elemWithSchema.Attribute("maxOccurs")?.Value);

        var elemWithSchemaType = Assert.Single(elemWithSchema.Elements());
        Assert.NotNull(elemWithSchemaType);
        Assert.Equal("complexType", elemWithSchemaType.Name.LocalName);

        var elemWithSchemaTypeAttr = Assert.Single(elemWithSchemaType.Elements());
        Assert.NotNull(elemWithSchemaTypeAttr);
        Assert.Equal("attribute", elemWithSchemaTypeAttr.Name.LocalName);
        Assert.Equal("TestAttribute", elemWithSchemaTypeAttr.Attribute("name")?.Value);
    }

    [Fact]
    public void ShouldOptOutOfXsd()
    {
        File.WriteAllText(
            xmlPath,
            """
            <Weavers GenerateXsd="false">
              <TestWeaver />
            </Weavers>
            """);

        var weavers = new[]
        {
            new WeaverEntry
            {
                AssemblyPath = @"something\TestWeaver.Fody.dll"
            }
        };

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, slnDir, testDir, new MockBuildLogger()).ToArray();
        ConfigFileFinder.EnsureSchemaIsUpToDate(testDir, weavers, true);

        Assert.Single(configFiles);
        Assert.Equal(xmlPath, configFiles[0].FilePath);

        Assert.False(File.Exists(xsdPath));

        var xml = XDocumentEx.Load(xmlPath);
        Assert.NotNull(xml.Root);
        Assert.Null(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation"));
    }

    [Fact]
    public void ShouldOptOutOfXsdThroughMSBuildProperty()
    {
        File.WriteAllText(xmlPath,
            """
            <Weavers>
              <TestWeaver />
            </Weavers>
            """);

        var weavers = new[]
        {
            new WeaverEntry
            {
                AssemblyPath = @"something\TestWeaver.Fody.dll"
            }
        };

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, slnDir, testDir, new MockBuildLogger()).ToArray();
        ConfigFileFinder.EnsureSchemaIsUpToDate(testDir, weavers, false);

        Assert.Single(configFiles);
        Assert.Equal(xmlPath, configFiles[0].FilePath);

        Assert.False(File.Exists(xsdPath));

        var xml = XDocumentEx.Load(xmlPath);
        Assert.NotNull(xml.Root);
        Assert.Null(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation"));
    }

    [Fact]
    public void ShouldNotCreateXsd_OnlySolutionWideConfig()
    {
        // Deliberately not writing the file in the project dir.
        if (File.Exists(xmlPath))
        {
            File.Delete(xmlPath);
        }

        File.WriteAllText(
            slnXmlPath,
            """
            <Weavers>
              <TestWeaver />
            </Weavers>
            """);

        var weavers = new[]
        {
            new WeaverEntry
            {
                AssemblyPath = @"something\TestWeaver.Fody.dll"
            }
        };

        var configFiles = ConfigFileFinder.FindWeaverConfigFiles(null, slnDir, testDir, new MockBuildLogger()).ToArray();
        ConfigFileFinder.EnsureSchemaIsUpToDate(testDir, weavers, true);

        Assert.Single(configFiles);
        Assert.Equal(slnXmlPath, configFiles[0].FilePath);

        Assert.False(File.Exists(slnXsdPath));

        var xml = XDocumentEx.Load(slnXmlPath);
        Assert.NotNull(xml.Root);
        Assert.Null(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation"));
    }

    [Fact]
    public void XmlConfigShouldOverrideMSBuildPropertyForXsdGeneration()
    {
        File.WriteAllText(
            xmlPath,
            """
            <Weavers GenerateXsd="true">
              <TestWeaver />
            </Weavers>
            """);

        var weavers = new[]
        {
            new WeaverEntry
            {
                AssemblyPath = @"something\TestWeaver.Fody.dll"
            }
        };

        var configs = ConfigFileFinder.FindWeaverConfigFiles(null, slnDir, testDir, new MockBuildLogger()).ToArray();
        ConfigFileFinder.EnsureSchemaIsUpToDate(testDir, weavers, false);

        Assert.Single(configs);
        Assert.Equal(xmlPath, configs[0].FilePath);

        Assert.True(File.Exists(xsdPath));

        var xml = XDocumentEx.Load(xmlPath);
        Assert.NotNull(xml.Root);
        Assert.Equal("FodyWeavers.xsd", xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")?.Value);
    }
}