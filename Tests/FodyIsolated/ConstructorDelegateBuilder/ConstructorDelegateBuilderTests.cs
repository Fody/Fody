public class ConstructorDelegateBuilderTests
{
    [Test]
    public async Task Should_throw_When_is_abstract_type()
    {
        var exception = await Assert.That(() => typeof (Convert).BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'System.Convert' is not a public instance class.");
    }

    [Test]
    public async Task Should_throw_When_is_abstract_static_type()
    {
        var exception = await Assert.That(() => typeof(Console).BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'System.Console' is not a public instance class.");
    }

    [Test]
    public async Task Should_throw_When_is_enum()
    {
        var exception = await Assert.That(() => typeof(AttributeTargets).BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'System.AttributeTargets' is not a public instance class.");
    }

    [Test]
    public async Task Should_throw_When_is_private()
    {
        var exception = await Assert.That(() => typeof(PrivateClass).BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'PrivateClass' is not a public instance class.");
    }

    [Test]
    public async Task Should_throw_When_is_internal()
    {
        var exception = await Assert.That(() => typeof(InternalClass).BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'InternalClass' is not a public instance class.");
    }

    [Test]
    public async Task Should_throw_When_has_parameters()
    {
        var type = typeof (WithParamsClass);
        var exception = await Assert.That(() => type.BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'WithParamsClass' does not have a public instance constructor with no parameters.");
    }

    [Test]
    public async Task Should_throw_When_is_nested()
    {
        var type = typeof(NestedPublicClass);
        var exception = await Assert.That(() => type.BuildConstructorDelegate()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("'ConstructorDelegateBuilderTests+NestedPublicClass' is a nested class which is not supported.");
    }

    [Test]
    public async Task Find_and_run()
    {
        var type = typeof(ValidClass);
        var anObject = type.BuildConstructorDelegate()();
        await Assert.That(anObject.GetType()).IsEqualTo(type);
    }

    [Test]
    public async Task Find_and_run_from_base()
    {
        var type = typeof(WeaverFromBase);
        var anObject = type.BuildConstructorDelegate()();
        await Assert.That(anObject.GetType()).IsEqualTo(type);
    }

    public class NestedPublicClass;
}