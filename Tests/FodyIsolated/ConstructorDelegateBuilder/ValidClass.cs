using System.Collections.Generic;
using Fody;

public class ValidClass:BaseModuleWeaver
{
    public override void Execute()
    {
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }
}