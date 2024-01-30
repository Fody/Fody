using System.Linq;

public class WeaverFromBase : BaseModuleWeaver
{
    public override void Execute()
    {
    }

    public override IEnumerable<string> GetAssembliesForScanning() =>
        Enumerable.Empty<string>();
}