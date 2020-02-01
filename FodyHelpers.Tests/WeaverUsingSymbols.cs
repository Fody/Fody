using System.Collections.Generic;
using System.Linq;
using Fody;
using Xunit;

public class WeaverUsingSymbols : BaseModuleWeaver
{
    public override void Execute()
    {
        var methods = ModuleDefinition.GetTypes().SelectMany(t => t.Methods).ToArray();

        Assert.NotNull(methods);
        Assert.True(methods.Any());

        var total = 0;

        foreach (var method in methods)
        {
            var sequencePoints = ModuleDefinition.SymbolReader.Read(method).SequencePoints;
            total += sequencePoints.Count;
        }

        Assert.True(total > 0);
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
    }
}