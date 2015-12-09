using System;
using NUnit.Framework;

[TestFixture]
public class PropertyDelegateBuilderTests : BaseClassSupplyingInheritedMembers
{
    public string Property { get; set; }
#pragma warning disable 169
    string privateField;
#pragma warning restore 169
// ReSharper disable UnusedMember.Local
    string PrivateProperty { get; set; }
// ReSharper restore UnusedMember.Local
// ReSharper disable UnassignedField.Global
    public string Field;
// ReSharper restore UnassignedField.Global
    public static string StaticField;

    [Test]
    public void Should_be_able_to_set_a_public_property_via_a_constructed_delegate()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("Property");
        setterDelegate(this, "aString");
        Assert.AreEqual("aString", Property);
    }
    
    [Test]
    public void Should_return_false_for_non_existing()
    {
        Action<object, string> setterDelegate;
        Assert.IsFalse(GetType().TryBuildPropertySetDelegate("NonExisting", out setterDelegate));
    }

    [Test]
    public void Should_be_able_to_set_a_public_field_via_a_constructed_delegate()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("Field");
        setterDelegate(this, "aString");
        Assert.AreEqual("aString",Field);
    }
    
    [Test]
    public void Should_be_able_to_set_inherited_properties() {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>(nameof(InheritedProperty));
        setterDelegate(this, "blah");
        Assert.AreEqual("blah", InheritedProperty);
    }

    [Test]
    public void Should_be_able_to_set_inherited_fields() {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>(nameof(InheritedField));
        setterDelegate(this, "blah");
        Assert.AreEqual("blah", InheritedField);
    }
    
    [Test]
    public void Should_be_a_null_delegate_When_member_does_not_exist()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("BadProperty");
        Assert.IsNull(setterDelegate.Target);
    }

    [Test]
    public void Should_be_a_null_delegate_When_private_property()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("privateField");
        Assert.IsNull(setterDelegate.Target);
    }

    [Test]
    public void Should_be_a_null_delegate_When_static_field()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("StaticField");
        Assert.IsNull(setterDelegate.Target);
    }

    [Test]
    public void Should_be_a_null_delegate_When_private_field()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("PrivateProperty");
        Assert.IsNull(setterDelegate.Target);
    }
    

}



public class BaseClassSupplyingInheritedMembers
{
    public string InheritedProperty { get; set; }
    public string InheritedField;
}
