public partial class InnerWeaver
{
    public List<string> SplitReferences = null!;

    public virtual void SplitUpReferences()
    {
        SplitReferences = References
            .Split([';'], StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        Logger.LogDebug("Reference count: " + SplitReferences.Count);

        var joinedReferences = string.Join(Environment.NewLine + "  ", SplitReferences.OrderBy(_ => _));
        Logger.LogDebug($"References:{Environment.NewLine}  {joinedReferences}");
    }
}
