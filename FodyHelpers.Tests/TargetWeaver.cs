using System.Collections.Generic;
using Fody;
using Xunit;

public class TargetWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var result = TryFindType("System.Boolean", out var type);
        Assert.True(result);
        Assert.NotNull(type);

        type = FindType("System.Boolean");
        Assert.NotNull(type);

        type = FindType("Boolean");
        Assert.NotNull(type);

        result = TryFindType("Boolean", out type);
        Assert.True(result);
        Assert.NotNull(type);

        result = TryFindType("DDD", out type);
        Assert.False(result);
        Assert.Null(type);
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
    }
}