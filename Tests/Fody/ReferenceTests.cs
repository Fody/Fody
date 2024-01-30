using System.Linq;

public class ReferenceTests
{
    [Fact]
    public void Ensure_fody_has_no_reference_to_Cecil()
    {
        var references = typeof(WeavingTask).Assembly
            .GetReferencedAssemblies()
            .Select(_ => _.Name)
            .ToList();
        Assert.DoesNotContain("Mono.Cecil", references);
    }
}