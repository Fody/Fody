using System;
using NUnit.Framework;

[TestFixture]
public class ExecuteDelegateBuilderTests
{

    [Test]
    [ExpectedException(ExpectedMessage = "'ExecuteDelegateBuilderTests+NoExecuteClass' must contain a public instance method named 'Execute'.")]
    public void Should_throw_When_no_execute_method()
    {
        typeof(NoExecuteClass).BuildExecuteDelegate();
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
    [ExpectedException(ExpectedMessage = "'ExecuteDelegateBuilderTests+NonPublicClass' must contain a public instance method named 'Execute'.")]
    public void Should_throw_When_execute_is_not_public()
    {
        typeof(NonPublicClass).BuildExecuteDelegate();
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
    [ExpectedException(ExpectedMessage = "'ExecuteDelegateBuilderTests+StaticExecuteClass' must contain a public instance method named 'Execute'.")]
    public void Should_thrown_When_method_is_static()
    {
        typeof(StaticExecuteClass).BuildExecuteDelegate();
    }

    public class StaticExecuteClass
    {
        public static void Execute()
        {
        }
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(NullReferenceException))]
    public void Should_thrown_inner_exception_When_delegate_is_executed()
    {
        var action = typeof (ThrowFromExecuteClass).BuildExecuteDelegate();
        action(new ThrowFromExecuteClass());
    }

    public class ThrowFromExecuteClass
    {
        public void Execute()
        {
            throw new NullReferenceException();
        }
    }

}