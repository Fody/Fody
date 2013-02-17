using System;
using NUnit.Framework;

[TestFixture]
public class InstanceConstructorTests
{
    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Convert' is not a public instance class.")]
	public void Should_throw_When_is_abstract_type()
    {
        typeof (Convert).ConstructInstance();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Console' is not a public instance class.")]
	public void Should_throw_When_is_abstract_static_type()
    {
        typeof(Console).ConstructInstance();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.AttributeTargets' is not a public instance class.")]
	public void Should_throw_When_is_enum()
    {
        typeof(AttributeTargets).ConstructInstance();
    }

    [Test]
	[ExpectedException(typeof(WeavingException), ExpectedMessage = "'PrivateClass' is not a public instance class.")]
	public void Should_throw_When_is_private()
    {
        typeof(PrivateClass).ConstructInstance();
    }


    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'InternalClass' is not a public instance class.")]
	public void Should_throw_When_is_internal()
    {
        typeof(InternalClass).ConstructInstance();
    }
    [Test]
	[ExpectedException(typeof(WeavingException), ExpectedMessage = "'WithParamsClass' does not have a public instance constructor with no parameters.")]
	public void Should_throw_When_has_parameters()
    {
	    var type = typeof (WithParamsClass);
	    type.ConstructInstance();
    }
    [Test]
	[ExpectedException(typeof(WeavingException), ExpectedMessage = "'InstanceConstructorTests+NestedPublicClass' is a nested class which is not supported.")]
	public void Should_throw_When_is_nested()
    {
		var type = typeof(NestedPublicClass);
	    type.ConstructInstance();
    }
    [Test]
	public void Should_construct_When_is_Valid()
    {
		var type = typeof(ValidClass);
	    Assert.IsNotNull(type.ConstructInstance()());
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