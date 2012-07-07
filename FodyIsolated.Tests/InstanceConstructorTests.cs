using System;
using NUnit.Framework;

[TestFixture]
public class InstanceConstructorTests
{
    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Convert' is not public instance class.")]
    public void AbstractType()
    {
        typeof (Convert).ConstructInstance();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.Console' is not public instance class.")]
    public void StaticType()
    {
        typeof(Console).ConstructInstance();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'System.AttributeTargets' is not public instance class.")]
    public void Enum()
    {
        typeof(AttributeTargets).ConstructInstance();
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'InstanceConstructorTests+PrivateClass' is not public instance class.")]
    public void Private()
    {
        typeof(PrivateClass).ConstructInstance();
    }

    class PrivateClass
    {
    }

    [Test]
    [ExpectedException(typeof(WeavingException), ExpectedMessage = "'InstanceConstructorTests+InternalClass' is not public instance class.")]
    public void Internal()
    {
        typeof(InternalClass).ConstructInstance();
    }

    internal class InternalClass
    {
    }

}