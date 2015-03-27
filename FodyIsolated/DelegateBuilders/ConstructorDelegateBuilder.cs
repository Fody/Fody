using System;
using System.Linq.Expressions;
using System.Reflection;

public static class ConstructorDelegateBuilder
{
    public static Func<object> BuildConstructorDelegate(this Type weaverType)
    {
        if (weaverType.IsNested)
        {
            throw new WeavingException(string.Format("'{0}' is a nested class which is not supported.", weaverType.FullName));
        }
        if (weaverType.IsAbstract || !weaverType.IsClass || !weaverType.IsPublic)
        {
            throw new WeavingException(string.Format("'{0}' is not a public instance class.", weaverType.FullName));
        }
        var constructorInfo = weaverType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] {}, null);
        if (constructorInfo == null)
        {
            var message = string.Format("'{0}' does not have a public instance constructor with no parameters.", weaverType.FullName);
            throw new WeavingException(message);
        }
        return (Func<object>) Expression.Lambda(Expression.Convert(Expression.New(constructorInfo), weaverType))
                                .Compile();
    }
}