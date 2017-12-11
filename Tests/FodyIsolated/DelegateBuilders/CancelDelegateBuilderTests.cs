using System;
using Xunit;

public class CancelDelegateBuilderTests : TestBase
{
    [Fact]
    public void Should_not_throw_When_no_cancel_method()
    {
        typeof(NoCancelClass).BuildCancelDelegate();
    }

    public class NoCancelClass
    {
    }

    [Fact]
    public void Should_find_method_When_cancel_is_valid()
    {
        typeof(ValidClass).BuildCancelDelegate()(new ValidClass());
    }

    public class ValidClass
    {
        public void Cancel()
        {
        }
    }

    [Fact]
    public void Should_not_throw_When_cancel_is_not_public()
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