using System.Collections.Generic;
using System.Linq;
using Fody;

public class WeaverUsingSymbols : BaseModuleWeaver
{
    public override void Execute()
    {
        var methods = ModuleDefinition.GetTypes().SelectMany(t => t.Methods).ToArray();
        Check(methods.Length > 0);

        var total = 0;

        foreach (var method in methods)
        {
            var sequencePoints = ModuleDefinition.SymbolReader.Read(method).SequencePoints;
            total += sequencePoints.Count;
        }

        Check(total > 0);
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