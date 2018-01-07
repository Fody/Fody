using System.Collections.Generic;
using System.Linq;
using Fody;

public class WeaverFromBase : BaseModuleWeaver
{
    public override void Execute()
    {
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }
}