using System;
using NUnit.Framework;

[TestFixture]
public class ConstructorDelegateBuilderTests
{

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Convert' is not a public instance class.")]
    public void Should_throw_When_is_abstract_type()
    {
        typeof (Convert).BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Console' is not a public instance class.")]
    public void Should_throw_When_is_abstract_static_type()
    {
        typeof(Console).BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.AttributeTargets' is not a public instance class.")]
    public void Should_throw_When_is_enum()
    {
        typeof(AttributeTargets).BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'PrivateClass' is not a public instance class.")]
    public void Should_throw_When_is_private()
    {
        typeof(PrivateClass).BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'InternalClass' is not a public instance class.")]
    public void Should_throw_When_is_internal()
    {
        typeof(InternalClass).BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'WithParamsClass' does not have a public instance constructor with no parameters.")]
    public void Should_throw_When_has_parameters()
    {
        var type = typeof (WithParamsClass);
        type.BuildConstructorDelegate();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'InstanceConstructorTests+ConstructorDelegateBuilderTests' is a nested class which is not supported.")]
    public void Should_throw_When_is_nested()
    {
        var type = typeof(NestedPublicClass);
        type.BuildConstructorDelegate();
    }
    [Test]
    public void Should_construct_When_is_Valid()
    {
        var type = typeof(ValidClass);
        Assert.IsNotNull(type.BuildConstructorDelegate()());
    }

    public class NestedPublicClass
    {
    }

}
public class WithParamsClass
{
    public WithParamsClass(string foo)
    {

    }
}
public class ValidClass
{
}
internal class InternalClass
{
}
class PrivateClass
{
}