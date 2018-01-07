using System;
using Fody;
using Xunit;

public class ExecuteDelegateBuilderTests : TestBase
{
    [Fact]
    public void Should_throw_When_no_method()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(NoExecuteClass).BuildExecuteDelegate());
        Assert.Equal("'ExecuteDelegateBuilderTests+NoExecuteClass' must contain a public instance method named 'Execute'.",exception.Message);
    }

    public class NoExecuteClass
    {
    }

    [Fact]
    public void Find_and_run()
    {
        var action = typeof(ValidClass).BuildExecuteDelegate();
        action(new ValidClass());
    }

    [Fact]
    public void Find_and_run_from_base()
    {
        var action = typeof(WeaverFromBase).BuildExecuteDelegate();
        action(new WeaverFromBase());
    }

    public class ValidClass
    {
        public void Execute()
        {
        }
    }

    [Fact]
    public void Should_throw_When_method_is_not_public()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(NonPublicClass).BuildExecuteDelegate());
        Assert.Equal("'ExecuteDelegateBuilderTests+NonPublicClass' must contain a public instance method named 'Execute'.", exception.Message);
    }

    public class NonPublicClass
    {
// ReSharper disable UnusedMember.Local
        void Execute()
// ReSharper restore UnusedMember.Local
        {
        }
    }

    [Fact]
    public void Should_thrown_When_method_is_static()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(StaticExecuteClass).BuildExecuteDelegate());
        Assert.Equal("'ExecuteDelegateBuilderTests+StaticExecuteClass' must contain a public instance method named 'Execute'.", exception.Message);
    }

    public class StaticExecuteClass
    {
        public static void Execute()
        {
        }
    }

    [Fact]
    public void Should_thrown_inner_exception_When_delegate_is_executed()
    {
        var action = typeof (ThrowFromExecuteClass).BuildExecuteDelegate();
        Assert.Throws<NullReferenceException>(() => action(new ThrowFromExecuteClass()));
    }

    public class ThrowFromExecuteClass
    {
        public void Execute()
        {
            throw new NullReferenceException();
        }
    }
}