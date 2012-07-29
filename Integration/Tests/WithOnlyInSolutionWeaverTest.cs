using System;
using NUnit.Framework;

[TestFixture]
public class WithOnlyInSolutionWeaverTest
{
    [Test]
    public void EnsureTypeInjectedByModuleWeaver()
    {
        Assert.IsNotNull(Type.GetType("Weavers.TypeInjectedByModuleWeaver, WithOnlyInSolutionWeaver"));
    }
}