using System.Xml.Linq;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var xElementFalse = XElement.Parse("<Node Name='false'/>");
        Assert.False(xElementFalse.ReadBool("Name", false));
        var xElement0 = XElement.Parse("<Node Name='0'/>");
        Assert.False(xElement0.ReadBool("Name", false));
        var xElementTrue = XElement.Parse("<Node Name='true'/>");
        Assert.True(xElementTrue.ReadBool("Name", false));
        var xElement1 = XElement.Parse("<Node Name='1'/>");
        Assert.True(xElement1.ReadBool("Name", false));
        var xElementNone = XElement.Parse("<Node />");
        Assert.False(xElementNone.ReadBool("Name", false));
        var xElementDefault = XElement.Parse("<Node/>");
        Assert.True(xElementDefault.ReadBool("Name", true));
    }

    [Fact]
    public void Whitespace()
    {
        var xElementFalse = XElement.Parse("<Node Name=' '/>");
        var exception = Assert.Throws<WeavingException>(() => xElementFalse.ReadBool("Name", false));
        ApprovalTests.Approvals.Verify(exception.Message);
    }

    [Fact]
    public void Empty()
    {
        var xElementFalse = XElement.Parse("<Node Name=''/>");
        var exception = Assert.Throws<WeavingException>(() => xElementFalse.ReadBool("Name", false));
        ApprovalTests.Approvals.Verify(exception.Message);
    }

    public ConfigReaderTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}