using System.Collections.Generic;
using Fody;

class PrivateClass: BaseModuleWeaver
{
    public override void Execute()
    {
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }
}