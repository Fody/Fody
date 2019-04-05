using System.Linq;
using Fody;
using Xunit;

public class ReferenceTests
{
    [Fact]
    public void Ensure_fody_has_no_reference_to_Cecil()
    {
        var references = typeof(WeavingTask).Assembly
            .GetReferencedAssemblies()
            .Select(x => x.Name)
            .ToList();
        Assert.DoesNotContain("Mono.Cecil", references);
    }
}