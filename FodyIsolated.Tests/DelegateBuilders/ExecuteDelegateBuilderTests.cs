using System;
using NUnit.Framework;

[TestFixture]
public class ExecuteDelegateBuilderTests
{

    [Test]
    public void Should_throw_When_no_execute_method()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(NoExecuteClass).BuildExecuteDelegate());
        Assert.AreEqual(exception.Message, "'ExecuteDelegateBuilderTests+NoExecuteClass' must contain a public instance method named 'Execute'.");
    }

    public class NoExecuteClass
    {
    }

    [Test]
    public void Should_find_method_When_execute_is_valid()
    {
        typeof(ValidClass).BuildExecuteDelegate()(new ValidClass());
    }

    public class ValidClass
    {
        public void Execute()
        {
        }
    }

    [Test]
    public void Should_throw_When_execute_is_not_public()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(NonPublicClass).BuildExecuteDelegate());
        Assert.AreEqual(exception.Message, "'ExecuteDelegateBuilderTests+NonPublicClass' must contain a public instance method named 'Execute'.");
    }

    public class NonPublicClass
    {
// ReSharper disable UnusedMember.Local
        void Execute()
// ReSharper restore UnusedMember.Local
        {
        }
    }

    [Test]
    public void Should_thrown_When_method_is_static()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(StaticExecuteClass).BuildExecuteDelegate());
        Assert.AreEqual(exception.Message, "'ExecuteDelegateBuilderTests+StaticExecuteClass' must contain a public instance method named 'Execute'.");
    }

    public class StaticExecuteClass
    {
        public static void Execute()
        {
        }
    }

    [Test]
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