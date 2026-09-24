using System.Linq;

public class ReferenceTests
{
    [Test]
    public async Task Ensure_fody_has_no_reference_to_Cecil()
    {
        var references = typeof(WeavingTask).Assembly
            .GetReferencedAssemblies()
            .Select(_ => _.Name)
            .ToList();
        await Assert.That(references).DoesNotContain("Mono.Cecil");
    }
}