using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class ReflectionTests
{
    public string Property { get; set; }
    [Test]
    public void Foo()
    {
        var makeSetterDelegate = GetType().BuildPropertyGetter<string>("Property");
        makeSetterDelegate(this, "sdfsdf");
        Assert.AreEqual("sdfsdf",Property);
    }

    public static Action<object, T> CreatePropertySetter<T>(Type targetType, string propertyName)
    {
        var property = targetType.GetProperty(propertyName);

        return PropertySetter<T>(targetType, property);
    }

    static Action<object, T> PropertySetter<T>(Type targetType, PropertyInfo property)
    {
        var target = Expression.Parameter(typeof (object));
        var value = Expression.Parameter(typeof (T));
        var body = Expression.Assign(
            Expression.Property(Expression.Convert(target, targetType), property),
            value);
        return Expression.Lambda<Action<object, T>>(body, target, value)
                         .Compile();
    }
}