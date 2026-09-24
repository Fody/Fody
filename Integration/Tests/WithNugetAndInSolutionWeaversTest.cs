using System;
using WithNugetAndInSolutionWeavers;

public class WithNugetAndInSolutionWeaversTest
{
    [Test]
    public async Task EnsureTypeInjectedByModuleWeaver()
    {
        await Assert.That(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithNugetAndInSolutionWeavers")).IsNotNull();
    }

    [Test]
    public async Task EnsureTypeInjectedByNamedWeaver()
    {
        await Assert.That(Type.GetType("Weavers.TypeInjectedByNamedWeaver, WithNugetAndInSolutionWeavers")).IsNotNull();
    }

    [Test]
    public async Task EnsureTypeInjectedByNamedWeaverFromBase()
    {
        await Assert.That(Type.GetType("Weavers.TypeInjectedByNamedWeaverFromBase, WithNugetAndInSolutionWeavers")).IsNotNull();
    }

    [Test]
    public async Task EnsureTypeChangedByNugetWeaver()
    {
        await Assert.That(typeof(Class1).GetMethod("Method").IsVirtual).IsTrue();
    }
}