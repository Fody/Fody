using System;

public class WithOnlyInSolutionWeaverTest
{
    [Test]
    public async Task EnsureTypeInjectedByModuleWeaver()
    {
        await Assert.That(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithOnlyInSolutionWeaver")).IsNotNull();
    }
}