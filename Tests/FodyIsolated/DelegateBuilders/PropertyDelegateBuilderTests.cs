using System;
using Xunit;

public class PropertyDelegateBuilderTests : TestBase
{
    [Fact]
    public void Should_be_able_to_set_a_public_property_via_a_constructed_delegate()
    {
        var target = new Target();
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("Property");
        setterDelegate(target, "aString");
        Assert.Equal("aString", target.Property);
    }

    [Fact]
    public void Should_return_false_for_non_existing()
    {
        Action<object, string> setterDelegate;
        Assert.False(typeof(Target).TryBuildPropertySetDelegate("NonExisting", out setterDelegate));
    }

    [Fact]
    public void Should_be_able_to_set_a_public_field_via_a_constructed_delegate()
    {
        var target = new Target();
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("Field");
        setterDelegate(target, "aString");
        Assert.Equal("aString", target.Field);
    }

    [Fact]
    public void Should_be_able_to_set_inherited_properties()
    {
        var target = new Target();
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>(nameof(Target.InheritedProperty));
        setterDelegate(target, "blah");
        Assert.Equal("blah", target.InheritedProperty);
    }

    [Fact]
    public void Should_be_able_to_set_inherited_fields()
    {
        var target = new Target();
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>(nameof(Target.InheritedField));
        setterDelegate(target, "blah");
        Assert.Equal("blah", target.InheritedField);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_member_does_not_exist()
    {
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("BadProperty");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_private_property()
    {
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("privateField");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_static_field()
    {
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("StaticField");
        Assert.Null(setterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_delegate_When_private_field()
    {
        var setterDelegate = typeof(Target).BuildPropertySetDelegate<string>("PrivateProperty");
        Assert.Null(setterDelegate.Target);
    }

    public class BaseClassSupplyingInheritedMembers
    {
        public string InheritedProperty { get; set; }
        public string InheritedField;
    }

    public class Target : BaseClassSupplyingInheritedMembers
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
    }
}