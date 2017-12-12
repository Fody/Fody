using System;
using Xunit;

public class WithOnlyInSolutionWeaverTest
{
    [Fact]
    public void EnsureTypeInjectedByModuleWeaver()
    {
        Assert.NotNull(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithOnlyInSolutionWeaver"));
    }
}