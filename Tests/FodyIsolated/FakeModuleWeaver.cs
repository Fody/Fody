using System.Collections.Generic;
using Fody;

public class FakeModuleWeaver :BaseModuleWeaver
{
    public override void Execute()
    {
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }
}