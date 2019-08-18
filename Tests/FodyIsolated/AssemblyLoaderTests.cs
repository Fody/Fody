using Xunit;
using Xunit.Abstractions;

public class AssemblyLoaderTests :
    XunitLoggingBase
{
    [Fact]
    public void TestsShouldReferenceTheCorrectVersionOfFodyHelpers()
    {
        Assert.Equal("FodyHelpers", InnerWeaver.FindFodyHelpersReference(GetType().Assembly).Name);
        InnerWeaver.ValidateWeaverReferencesAtLeastTheCurrentMajorOfFody(GetType().Assembly);
    }

    public AssemblyLoaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}