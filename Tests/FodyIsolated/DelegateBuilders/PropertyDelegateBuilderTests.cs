using System;
using Xunit;

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

    [Fact]
    public void Should_be_able_to_set_a_public_property_via_a_constructed_delegate()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("Property");
        setterDelegate(this, "aString");
        Assert.Equal("aString", Property);
    }

    [Fact]
    public void Should_return_false_for_non_existing()
    {
        Action<object, string> setterDelegate;
        Assert.False(GetType().TryBuildPropertySetDelegate("NonExisting", out setterDelegate));
    }

    [Fact]
    public void Should_be_able_to_set_a_public_field_via_a_constructed_delegate()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("Field");
        setterDelegate(this, "aString");
        Assert.Equal("aString",Field);
    }

    [Fact]
    public void Should_be_able_to_set_inherited_properties() {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>(nameof(InheritedProperty));
        setterDelegate(this, "blah");
        Assert.Equal("blah", InheritedProperty);
    }

    [Fact]
    public void Should_be_able_to_set_inherited_fields() {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>(nameof(InheritedField));
        setterDelegate(this, "blah");
        Assert.Equal("blah", InheritedField);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_member_does_not_exist()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("BadProperty");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_private_property()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("privateField");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_static_field()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("StaticField");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_private_field()
    {
        var setterDelegate = GetType().BuildPropertySetDelegate<string>("PrivateProperty");
        Assert.Null(setterDelegate.Target);
    }
}