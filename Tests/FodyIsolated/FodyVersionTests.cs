public class FodyVersionTests
{
    [Fact]
    public void FindFodyHelpersReference() =>
        Assert.Equal("FodyHelpers", FodyVersion.FindFodyHelpersReference(GetType().Assembly).Name);
}