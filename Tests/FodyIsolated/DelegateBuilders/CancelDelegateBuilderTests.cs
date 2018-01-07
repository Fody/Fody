using System;
using Xunit;

public class CancelDelegateBuilderTests : TestBase
{
    [Fact]
    public void Should_not_throw_When_no_method()
    {
        typeof(NoCancelClass).BuildCancelDelegate();
    }

    public class NoCancelClass
    {
    }

    [Fact]
    public void Find_and_run()
    {
        var action = typeof(ValidClass).BuildCancelDelegate();
        action(new ValidClass());
    }

    [Fact]
    public void Find_and_run_from_base()
    {
        var action = typeof(WeaverFromBase).BuildCancelDelegate();
        action(new WeaverFromBase());
    }

    public class ValidClass
    {
        public void Cancel()
        {
        }
    }

    [Fact]
    public void Should_not_throw_When_method_is_not_public()
    {
        typeof(NonPublicClass).BuildCancelDelegate();
    }

    public class NonPublicClass
    {
// ReSharper disable UnusedMember.Local
        void Cancel()
// ReSharper restore UnusedMember.Local
        {
        }
    }

    [Fact]
    public void Should_not_throw_When_method_is_static()
    {
        typeof(StaticExecuteClass).BuildCancelDelegate();
    }

    public class StaticExecuteClass
    {
        public static void Cancel()
        {
        }
    }

    [Fact]
    public void Should_thrown_inner_exception_When_delegate_is_executed()
    {
        var action = typeof (ThrowFromExecuteClass).BuildCancelDelegate();
        Assert.Throws<NullReferenceException>(() => action(new ThrowFromExecuteClass()));
    }

    public class ThrowFromExecuteClass
    {
        public void Cancel()
        {
            throw new NullReferenceException();
        }
    }
}