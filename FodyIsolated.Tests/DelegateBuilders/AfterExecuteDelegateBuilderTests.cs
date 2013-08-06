using System;
using NUnit.Framework;

[TestFixture]
public class AfterExecuteDelegateBuilderTests
{

    [Test]
    public void Should_not_throw_When_no_execute_method()
    {
        typeof(NoExecuteClass).BuildAfterWeavingDelegate();
    }

    public class NoExecuteClass
    {
    }

    [Test]
    public void Should_find_method_When_execute_is_valid()
    {
        typeof(ValidClass).BuildAfterWeavingDelegate()(new ValidClass());
    }

    public class ValidClass
    {
        public void AfterWeavingExecute()
        {
        }
    }

    [Test]
    public void Should_not_throw_When_execute_is_not_public()
    {
        typeof(NonPublicClass).BuildAfterWeavingDelegate();
    }

    public class NonPublicClass
    {
// ReSharper disable UnusedMember.Local
        void AfterWeavingExecute()
// ReSharper restore UnusedMember.Local
        {
        }
    }

    [Test]
    public void Should_not_throw_When_method_is_static()
    {
        typeof(StaticExecuteClass).BuildAfterWeavingDelegate();
    }

    public class StaticExecuteClass
    {
        public static void AfterWeavingExecute()
        {
        }
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(NullReferenceException))]
    public void Should_thrown_inner_exception_When_delegate_is_executed()
    {
        var action = typeof(ThrowFromExecuteClass).BuildAfterWeavingDelegate();
        action(new ThrowFromExecuteClass());
    }

    public class ThrowFromExecuteClass
    {
        public void AfterWeavingExecute()
        {
            throw new NullReferenceException();
        }
    }

}