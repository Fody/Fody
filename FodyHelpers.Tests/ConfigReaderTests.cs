using System.Threading.Tasks;
using System.Xml.Linq;
using Fody;
using VerifyTUnit;

public class ConfigReaderTests
{
    [Test]
    public async Task Simple()
    {
        var xElementFalse = XElement.Parse("<Node Name='false'/>");
        await Assert.That(xElementFalse.ReadBool("Name", false)).IsFalse();
        var xElement0 = XElement.Parse("<Node Name='0'/>");
        await Assert.That(xElement0.ReadBool("Name", false)).IsFalse();
        var xElementTrue = XElement.Parse("<Node Name='true'/>");
        await Assert.That(xElementTrue.ReadBool("Name", false)).IsTrue();
        var xElement1 = XElement.Parse("<Node Name='1'/>");
        await Assert.That(xElement1.ReadBool("Name", false)).IsTrue();
        var xElementTrueMixedCase = XElement.Parse("<Node Name='True'/>");
        await Assert.That(xElementTrueMixedCase.ReadBool("Name", false)).IsTrue();
        var xElementFalseMixedCase = XElement.Parse("<Node Name='False'/>");
        await Assert.That(xElementFalseMixedCase.ReadBool("Name", true)).IsFalse();
        var xElementTrueUpperCase = XElement.Parse("<Node Name='TRUE'/>");
        await Assert.That(xElementTrueUpperCase.ReadBool("Name", false)).IsTrue();
        var xElementNone = XElement.Parse("<Node />");
        await Assert.That(xElementNone.ReadBool("Name", false)).IsFalse();
        var xElementDefault = XElement.Parse("<Node/>");
        await Assert.That(xElementDefault.ReadBool("Name", true)).IsTrue();
    }

    [Test]
    public async Task Whitespace()
    {
        var xElementFalse = XElement.Parse("<Node Name=' '/>");
        var exception = await Assert.That(() => xElementFalse.ReadBool("Name", false)).Throws<WeavingException>();
        await Verifier.Verify(exception!.Message);
    }

    [Test]
    public async Task Empty()
    {
        var xElementFalse = XElement.Parse("<Node Name=''/>");
        var exception = await Assert.That(() => xElementFalse.ReadBool("Name", false)).Throws<WeavingException>();
        await Verifier.Verify(exception!.Message);
    }
}