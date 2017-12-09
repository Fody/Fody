using System;
using NUnit.Framework;

[TestFixture]
public class ConstructorDelegateBuilderTests
{
    [Test]
    public void Should_throw_When_is_abstract_type()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof (Convert).BuildConstructorDelegate());
        Assert.AreEqual(exception.Message,"'System.Convert' is not a public instance class.");
    }

    [Test]
    public void Should_throw_When_is_abstract_static_type()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(Console).BuildConstructorDelegate());
        Assert.AreEqual(exception.Message, "'System.Console' is not a public instance class.");
    }

    [Test]
    public void Should_throw_When_is_enum()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(AttributeTargets).BuildConstructorDelegate());
        Assert.AreEqual(exception.Message, "'System.AttributeTargets' is not a public instance class.");
    }

    [Test]
    public void Should_throw_When_is_private()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(PrivateClass).BuildConstructorDelegate());
        Assert.AreEqual(exception.Message,"'PrivateClass' is not a public instance class.");
    }

    [Test]
    public void Should_throw_When_is_internal()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(InternalClass).BuildConstructorDelegate());
        Assert.AreEqual(exception.Message, "'InternalClass' is not a public instance class.");
    }

    [Test]
    public void Should_throw_When_has_parameters()
    {
        var type = typeof (WithParamsClass);
        var exception = Assert.Throws<WeavingException>(() => type.BuildConstructorDelegate());
        Assert.AreEqual(exception.Message, "'WithParamsClass' does not have a public instance constructor with no parameters.");
    }

    [Test]
    public void Should_throw_When_is_nested()
    {
        var type = typeof(NestedPublicClass);
        var exception = Assert.Throws<WeavingException>(() => type.BuildConstructorDelegate());
        Assert.AreEqual(exception.Message, "'ConstructorDelegateBuilderTests+NestedPublicClass' is a nested class which is not supported.");
    }

    [Test]
    public void Should_construct_When_is_Valid()
    {
        var type = typeof(ValidClass);
        var anObject = type.BuildConstructorDelegate()();
        Assert.IsNotNull(anObject);
        Assert.AreEqual(type,anObject.GetType());
    }

    public class NestedPublicClass
    {
    }
}

public class WithParamsClass
{
    // ReSharper disable once UnusedParameter.Local
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