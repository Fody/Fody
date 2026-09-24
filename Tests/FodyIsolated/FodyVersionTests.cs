public class FodyVersionTests
{
    [Test]
    public async Task FindFodyHelpersReference() =>
        await Assert.That(FodyVersion.FindFodyHelpersReference(GetType().Assembly).Name).IsEqualTo("FodyHelpers");
}