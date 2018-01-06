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

    [Fact]
    public void Should_be_able_to_get_a_public_property_via_a_constructed_delegate()
    {
        var target = new Target { Property = "aString" };
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("Property");
        string value = getterDelegate(target);
        Assert.Equal(target.Property, value);
    }

    [Fact]
    public void Getter_should_return_false_for_non_existing()
    {
        Assert.False(typeof(Target).TryBuildPropertyGetDelegate("NonExisting", out Func<object, string> getterDelegate));
    }

    [Fact]
    public void Should_be_able_to_get_a_public_field_via_a_constructed_delegate()
    {
        var target = new Target { Field = "aString" };
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("Field");
        string value = getterDelegate(target);
        Assert.Equal(target.Field, value);
    }

    [Fact]
    public void Should_be_able_to_get_inherited_properties()
    {
        var target = new Target { InheritedProperty = "blah" };
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>(nameof(Target.InheritedProperty));
        string value = getterDelegate(target);
        Assert.Equal(target.InheritedProperty, value);
    }

    [Fact]
    public void Should_be_able_to_get_inherited_fields()
    {
        var target = new Target { InheritedField = "blah" };
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>(nameof(Target.InheritedField));
        string value = getterDelegate(target);
        Assert.Equal(target.InheritedField, value);
    }

    [Fact]
    public void Should_be_a_null_getter_delegate_When_member_does_not_exist()
    {
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("BadProperty");
        Assert.Null(getterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_getter_delegate_When_private_property()
    {
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("privateField");
        Assert.Null(getterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_getter_delegate_When_static_field()
    {
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("StaticField");
        Assert.Null(getterDelegate.Target);
    }

    [Fact]
    public void Should_be_a_null_getter_delegate_When_private_field()
    {
        var getterDelegate = typeof(Target).BuildPropertyGetDelegate<string>("PrivateProperty");
        Assert.Null(getterDelegate.Target);
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