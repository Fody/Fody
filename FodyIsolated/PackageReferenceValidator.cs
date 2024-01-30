public partial class InnerWeaver
{
    static void ValidatePackageReferenceSettings(IList<WeaverHolder> weaverInstances, ILogger logger)
    {
        var weaversWithoutReference = weaverInstances.Where(weaver => weaver.Instance.ShouldCleanReference).ToList();

        if (!weaversWithoutReference.Any())
        {
            logger.LogDebug("No weaver with 'ShouldCleanReference=true' found, skip package reference validation");
            return;
        }

        var weaverNames = weaversWithoutReference
            .Select(weaver => weaver.Config.WeaverName)
            .ToList();

        logger.LogDebug($"Weavers with 'ShouldCleanReference=true': {string.Join(", ", weaverNames)}");

        foreach (var weaverInstance in weaversWithoutReference)
        {
            var errors = GetPackageReferenceValidationErrors(weaverInstance.Config);

            foreach (var error in errors)
            {
                logger.LogWarning(error, "FodyPackageReference");
            }
        }
    }

    public static IEnumerable<string> GetPackageReferenceValidationErrors(WeaverEntry weaver)
    {
        if (!weaver.HasPackageReference)
        {
            yield break;
        }

        if (!string.Equals(weaver.PrivateAssets, "All", StringComparison.OrdinalIgnoreCase))
        {
            yield return $"The package reference for {weaver.WeaverName} does not contain PrivateAssets='All'";
        }

        if (weaver.IncludeAssets != string.Empty
            && !string.Equals(weaver.IncludeAssets, "All", StringComparison.OrdinalIgnoreCase)
            && !weaver.IncludeAssets!.Split(';').Select(item => item.Trim()).Contains("compile", StringComparer.OrdinalIgnoreCase))
        {
            yield return $"The package reference for {weaver.WeaverName} is missing the 'compile' part in the IncludeAssets setting; it's recommended to completely remove IncludeAssets";
        }
    }
}
