using System;
using Xunit;
using WithNugetAndInSolutionWeavers;

public class WithNugetAndInSolutionWeaversTest
{
    [Fact]
    public void EnsureTypeInjectedByModuleWeaver()
    {
        Assert.NotNull(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithNugetAndInSolutionWeavers"));
    }

    [Fact]
    public void EnsureTypeInjectedByNamedWeaver()
    {
        Assert.NotNull(Type.GetType("Weavers.TypeInjectedByNamedWeaver, WithNugetAndInSolutionWeavers"));
    }

    [Fact]
    public void EnsureTypeInjectedByNamedWeaverFromBase()
    {
        Assert.NotNull(Type.GetType("Weavers.TypeInjectedByNamedWeaverFromBase, WithNugetAndInSolutionWeavers"));
    }

    [Fact]
    public void EnsureTypeChangedByNugetWeaver()
    {
        Assert.True(typeof(Class1).GetMethod("Method").IsVirtual);
    }
}