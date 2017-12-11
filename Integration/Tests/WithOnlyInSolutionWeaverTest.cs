using System;
using Xunit;

public class WithOnlyInSolutionWeaverTest
{
    [Test]
    public void EnsureTypeInjectedByModuleWeaver()
    {
        Assert.IsNotNull(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithOnlyInSolutionWeaver"));
    }
}