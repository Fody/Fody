using NUnit.Framework;

[TestFixture]
public class PropertyDelegateBuilderTests
{
    public string Property { get; set; }

    [Test]
    public void Should_be_able_to_set_a_public_property_via_a_contructed_delegate()
    {
        var makeSetterDelegate = GetType().BuildPropertySetDelegate<string>("Property");
        makeSetterDelegate(this, "sdfsdf");
        Assert.AreEqual("sdfsdf",Property);
    }

    [Test]
    public void Should_return_null_When_property_doesnt_exist()
    {
		var setMEthod = GetType().GetPropertySetMethod<string>("BadProperty");
        Assert.IsNull(setMEthod);
    }


}