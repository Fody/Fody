namespace Tests.FodyIsolated;

public class ValidatePackageReferenceSettingsTests
{
    [Theory]
    [InlineData(null, null, "")]
    [InlineData("All", "", "")]
    [InlineData("all", "", "")]
    [InlineData("", "", "The package reference for Weaver.Fody does not contain PrivateAssets='All'")]
    [InlineData("All", "All", "")]
    [InlineData("All", "all", "")]
    [InlineData("All", "runtime; build; compile; native; contentfiles; analyzers; buildtransitive", "")]
    [InlineData("None", "", "The package reference for Weaver.Fody does not contain PrivateAssets='All'")]
    [InlineData("All", "runtime; build; native; contentfiles; analyzers; buildtransitive", "The package reference for Weaver.Fody is missing the 'compile' part in the IncludeAssets setting; it's recommended to completely remove IncludeAssets")]
    void Test(string? privateAssets, string? includeAssets, string expectedErrors)
    {
        var config = new WeaverEntry
        {
            AssemblyPath = "Weaver.Fody.dll",
            PrivateAssets = privateAssets,
            IncludeAssets = includeAssets
        };

        var errors = InnerWeaver.GetPackageReferenceValidationErrors(config);

        Assert.Equal(expectedErrors, string.Join("|", errors));
    }
}