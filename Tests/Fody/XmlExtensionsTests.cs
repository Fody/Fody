using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

public class XmlExtensionsTests
{
    [Fact]
    public void Simple()
    {
        var xDocument = XDocument.Parse(
            """
            <root>
              <f:table xmlns:f="http://www.w3schools.com/furniture">
                <f:name xml:id="1" xml:id2="2">African Coffee Table</f:name>
                <f:width>80</f:width>
                <f:length>120</f:length>
              </f:table>
            </root>
            """);
        xDocument.StripNamespace();
        Assert.Equal(
            """
            <root>
              <table>
                <name id="1" id2="2">African Coffee Table</name>
                <width>80</width>
                <length>120</length>
              </table>
            </root>
            """.Replace("\r\n", "\n"), xDocument.ToString().Replace("\r\n", "\n"));
    }

    [Fact]
    public void QueryWithNamespace()
    {
        var xDocument = XDocument.Parse(
            """
            <root>
              <f:table xmlns:f="http://www.w3schools.com/furniture">
                <f:name>African Coffee Table</f:name>
                <f:width>80</f:width>
                <f:length>120</f:length>
              </f:table>
            </root>
            """);
        var qualifiedName = XName.Get("table", "http://www.w3schools.com/furniture");
        var tables = xDocument.Descendants(qualifiedName);
        Trace.WriteLine(tables.Count());
    }

    [Fact]
    public void QueryWithNoNamespace()
    {
        var xDocument = XDocument.Parse(
            """
            <root>
              <f:table xmlns:f="http://www.w3schools.com/furniture">
                <f:name>African Coffee Table</f:name>
                <f:width>80</f:width>
                <f:length>120</f:length>
              </f:table>
            </root>
            """);
        xDocument.StripNamespace();
        var tables = xDocument.Descendants("table");
        Trace.WriteLine(tables.Count());
    }
}