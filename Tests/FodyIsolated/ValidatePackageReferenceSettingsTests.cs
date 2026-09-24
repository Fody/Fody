namespace Tests.FodyIsolated;

public class ValidatePackageReferenceSettingsTests
{
    [Test]
    [Arguments(null, null, "")]
    [Arguments("All", "", "")]
    [Arguments("all", "", "")]
    [Arguments("", "", "The package reference for Weaver.Fody does not contain PrivateAssets='All'")]
    [Arguments("All", "All", "")]
    [Arguments("All", "all", "")]
    [Arguments("All", "runtime; build; compile; native; contentfiles; analyzers; buildtransitive", "")]
    [Arguments("None", "", "The package reference for Weaver.Fody does not contain PrivateAssets='All'")]
    [Arguments("All", "runtime; build; native; contentfiles; analyzers; buildtransitive", "The package reference for Weaver.Fody is missing the 'compile' part in the IncludeAssets setting; it's recommended to completely remove IncludeAssets")]
    public async Task Test(string? privateAssets, string? includeAssets, string expectedErrors)
    {
        var config = new WeaverEntry
        {
            AssemblyPath = "Weaver.Fody.dll",
            PrivateAssets = privateAssets,
            IncludeAssets = includeAssets
        };

        var errors = InnerWeaver.GetPackageReferenceValidationErrors(config);

        await Assert.That(string.Join("|", errors)).IsEqualTo(expectedErrors);
    }
}