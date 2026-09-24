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

    [Test]
    public async Task ShouldCreateXsd()
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
        var logger = new MockBuildLogger();

        ConfigFileFinder.EnsureSchemaIsUpToDate(logger, testDir, weavers, true);

        await Assert.That(configFiles).HasSingleItem();
        await Assert.That(configFiles[0].AllowExtraEntries).IsFalse();
        await Assert.That(configFiles[0].FilePath).IsEqualTo(xmlPath);

        await Assert.That(File.Exists(xsdPath)).IsTrue();

        var xml = XDocumentEx.Load(xmlPath);
        await Assert.That(xml.Root).IsNotNull();
        await Assert.That(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")?.Value).IsEqualTo("FodyWeavers.xsd");

        var xsd = XDocumentEx.Load(xsdPath);
        await Assert.That(xsd.Root).IsNotNull();
        var elements = xsd.Root.Descendants(schemaNamespace + "all").First().Elements().ToList();

        await Assert.That(elements.Count).IsEqualTo(2);

        var defaultElem = elements[0];
        await Assert.That(defaultElem.Name.LocalName).IsEqualTo("element");
        await Assert.That(defaultElem.Attribute("name")?.Value).IsEqualTo("TestWeaver");
        await Assert.That(defaultElem.Attribute("type")?.Value).IsEqualTo("xs:anyType");
        await Assert.That(defaultElem.Attribute("minOccurs")?.Value).IsEqualTo("0");
        await Assert.That(defaultElem.Attribute("maxOccurs")?.Value).IsEqualTo("1");

        var elemWithSchema = elements[1];
        await Assert.That(elemWithSchema.Name.LocalName).IsEqualTo("element");
        await Assert.That(elemWithSchema.Attribute("name")?.Value).IsEqualTo("WeaverWithSchema");
        await Assert.That(elemWithSchema.Attribute("type")).IsNull();
        await Assert.That(elemWithSchema.Attribute("minOccurs")?.Value).IsEqualTo("0");
        await Assert.That(elemWithSchema.Attribute("maxOccurs")?.Value).IsEqualTo("1");

        var elemWithSchemaType = await Assert.That(elemWithSchema.Elements()).HasSingleItem();
        await Assert.That(elemWithSchemaType).IsNotNull();
        await Assert.That(elemWithSchemaType.Name.LocalName).IsEqualTo("complexType");

        var elemWithSchemaTypeAttr = await Assert.That(elemWithSchemaType.Elements()).HasSingleItem();
        await Assert.That(elemWithSchemaTypeAttr).IsNotNull();
        await Assert.That(elemWithSchemaTypeAttr.Name.LocalName).IsEqualTo("attribute");
        await Assert.That(elemWithSchemaTypeAttr.Attribute("name")?.Value).IsEqualTo("TestAttribute");
    }

    [Test]
    public async Task ShouldOptOutOfXsd()
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
        var logger = new MockBuildLogger();

        ConfigFileFinder.EnsureSchemaIsUpToDate(logger, testDir, weavers, true);

        await Assert.That(configFiles).HasSingleItem();
        await Assert.That(configFiles[0].FilePath).IsEqualTo(xmlPath);

        await Assert.That(File.Exists(xsdPath)).IsFalse();

        var xml = XDocumentEx.Load(xmlPath);
        await Assert.That(xml.Root).IsNotNull();
        await Assert.That(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")).IsNull();
    }

    [Test]
    public async Task ShouldOptOutOfXsdThroughMSBuildProperty()
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
        var logger = new MockBuildLogger();

        ConfigFileFinder.EnsureSchemaIsUpToDate(logger, testDir, weavers, false);

        await Assert.That(configFiles).HasSingleItem();
        await Assert.That(configFiles[0].FilePath).IsEqualTo(xmlPath);

        await Assert.That(File.Exists(xsdPath)).IsFalse();

        var xml = XDocumentEx.Load(xmlPath);
        await Assert.That(xml.Root).IsNotNull();
        await Assert.That(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")).IsNull();
    }

    [Test]
    public async Task ShouldNotCreateXsd_OnlySolutionWideConfig()
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
        var logger = new MockBuildLogger();

        ConfigFileFinder.EnsureSchemaIsUpToDate(logger, testDir, weavers, true);

        await Assert.That(configFiles).HasSingleItem();
        await Assert.That(configFiles[0].FilePath).IsEqualTo(slnXmlPath);

        await Assert.That(File.Exists(slnXsdPath)).IsFalse();

        var xml = XDocumentEx.Load(slnXmlPath);
        await Assert.That(xml.Root).IsNotNull();
        await Assert.That(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")).IsNull();
    }

    [Test]
    public async Task XmlConfigShouldOverrideMSBuildPropertyForXsdGeneration()
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
        var logger = new MockBuildLogger();

        ConfigFileFinder.EnsureSchemaIsUpToDate(logger, testDir, weavers, false);

        await Assert.That(configs).HasSingleItem();
        await Assert.That(configs[0].FilePath).IsEqualTo(xmlPath);

        await Assert.That(File.Exists(xsdPath)).IsTrue();

        var xml = XDocumentEx.Load(xmlPath);
        await Assert.That(xml.Root).IsNotNull();
        await Assert.That(xml.Root.Attribute(schemaInstanceNamespace + "noNamespaceSchemaLocation")?.Value).IsEqualTo("FodyWeavers.xsd");
    }
}