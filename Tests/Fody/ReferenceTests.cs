using System.Linq;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ReferenceTests :
    VerifyBase
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

    public ReferenceTests(ITestOutputHelper output) :
        base(output)
    {
    }
}