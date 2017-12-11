using System;
using Xunit;

public class ConstructorDelegateBuilderTests : TestBase
{
    [Fact]
    public void Should_throw_When_is_abstract_type()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof (Convert).BuildConstructorDelegate());
        Assert.Equal("'System.Convert' is not a public instance class.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_is_abstract_static_type()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(Console).BuildConstructorDelegate());
        Assert.Equal("'System.Console' is not a public instance class.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_is_enum()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(AttributeTargets).BuildConstructorDelegate());
        Assert.Equal("'System.AttributeTargets' is not a public instance class.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_is_private()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(PrivateClass).BuildConstructorDelegate());
        Assert.Equal("'PrivateClass' is not a public instance class.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_is_internal()
    {
        var exception = Assert.Throws<WeavingException>(() => typeof(InternalClass).BuildConstructorDelegate());
        Assert.Equal("'InternalClass' is not a public instance class.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_has_parameters()
    {
        var type = typeof (WithParamsClass);
        var exception = Assert.Throws<WeavingException>(() => type.BuildConstructorDelegate());
        Assert.Equal("'WithParamsClass' does not have a public instance constructor with no parameters.", exception.Message);
    }

    [Fact]
    public void Should_throw_When_is_nested()
    {
        var type = typeof(NestedPublicClass);
        var exception = Assert.Throws<WeavingException>(() => type.BuildConstructorDelegate());
        Assert.Equal("'ConstructorDelegateBuilderTests+NestedPublicClass' is a nested class which is not supported.", exception.Message);
    }

    [Fact]
    public void Should_construct_When_is_Valid()
    {
        var type = typeof(ValidClass);
        var anObject = type.BuildConstructorDelegate()();
        Assert.NotNull(anObject);
        Assert.Equal(type, anObject.GetType());
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