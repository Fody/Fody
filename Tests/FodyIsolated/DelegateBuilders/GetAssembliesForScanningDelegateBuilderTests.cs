using System;
using System.Collections.Generic;
using Fody;
using Xunit;

public class GetAssembliesForScanningDelegateBuilderTests : TestBase
{
    [Fact]
    public void Find_and_run_from_base()
    {
        var action = typeof(WeaverFromBase).BuildGetAssembliesForScanningDelegate();
        action(new ValidClass());
    }

    public class ValidClass : BaseModuleWeaver
    {
        public override void Execute()
        {
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield break;
        }
    }

    [Fact]
    public void Should_thrown_inner_exception_When_delegate_is_executed()
    {
        var action = typeof (ThrowFromExecuteClass).BuildGetAssembliesForScanningDelegate();
        Assert.Throws<NullReferenceException>(() => action(new ThrowFromExecuteClass()));
    }

    public class ThrowFromExecuteClass: BaseModuleWeaver
    {
        public override void Execute()
        {
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            throw new NullReferenceException();
        }
    }
}