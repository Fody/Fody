using System;
using System.Linq.Expressions;
using System.Reflection;
using Fody;

public static class ConstructorDelegateBuilder
{
    public static Func<object> BuildConstructorDelegate(this Type weaverType)
    {
        if (weaverType.IsNested)
        {
            throw new WeavingException($"'{weaverType.FullName}' is a nested class which is not supported.");
        }
        if (weaverType.IsAbstract || !weaverType.IsClass || !weaverType.IsPublic)
        {
            throw new WeavingException($"'{weaverType.FullName}' is not a public instance class.");
        }
        var constructorInfo = weaverType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] {}, null);
        if (constructorInfo == null)
        {
            var message = $"'{weaverType.FullName}' does not have a public instance constructor with no parameters.";
            throw new WeavingException(message);
        }
        return (Func<object>) Expression.Lambda(Expression.Convert(Expression.New(constructorInfo), weaverType))
                                .Compile();
    }
}