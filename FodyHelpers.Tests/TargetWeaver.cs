using System.Collections.Generic;
using Fody;

public class TargetWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var result = TryFindType("System.Boolean", out var type);
        Check(result);
        Check(type != null);

        type = FindType("System.Boolean");
        Check(type != null);

        type = FindType("Boolean");
        Check(type != null);

        result = TryFindType("Boolean", out type);
        Check(result);
        Check(type != null);

        result = TryFindType("DDD", out type);
        Check(!result);
        Check(type == null);
    }

    static void Check(bool condition)
    {
        if (!condition)
        {
            throw new WeavingException("Weaver assertion failed");
        }
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
    }
}