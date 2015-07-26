using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

public static class PropertyDelegateBuilder
{
    public static Action<object, T> BuildPropertySetDelegate<T>(this Type targetType, string propertyName)
    {
        Action<object, T> action;
        TryBuildPropertySetDelegate(targetType, propertyName, out action);
        return action;
    }

    public static bool TryBuildPropertySetDelegate<T>(this Type targetType, string propertyName, out Action<object, T> action)
    {
        var propertyInfo = targetType.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof(T), new Type[] { }, null);

        if (propertyInfo != null)
        {
            var setMethod = propertyInfo.GetSetMethod();

            Action<object, T> setter = (instance, val) => DelegateHelper.CreateCompatibleDelegate<Action<T>>(instance, setMethod)(val);
            action = setter;

            return true;
        }

        var fieldInfo = GetField<T>(targetType, propertyName);
        if (fieldInfo != null)
        {
            var target = Expression.Parameter(typeof (object), "target");
            var value = Expression.Parameter(typeof (T), "value");
            var fieldExp = Expression.Field(Expression.Convert(target, targetType), fieldInfo);
            var body = AssignmentExpression.Create(fieldExp, value); // Expression.Assign(fieldExp, value);
            action= Expression.Lambda<Action<object, T>>(body, target, value)
                             .Compile();
            return true;

        }
        action= (x, y) => { };
        return false;
    }

    public static FieldInfo GetField<TField>(this Type type, string propertyName)
    {
        var fieldInfo = type.GetField(propertyName);
        if (fieldInfo == null)
        {
            return null;
        }
        if (!fieldInfo.IsPublic)
        {
            return null;
        }
        if (fieldInfo.IsStatic)
        {
            return null;
        }
        if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        {
            return null;
        }
        return fieldInfo;
    }

   
    private static class AssignmentExpression
    {
        public static Expression Create(Expression left, Expression right)
        {
            return
                Expression.Call(
                   null,
                   typeof(AssignmentExpression)
                      .GetMethod("AssignTo", BindingFlags.NonPublic | BindingFlags.Static)
                      .MakeGenericMethod(left.Type),
                   left,
                   right);
        }

        private static void AssignTo<T>(ref T left, T right)  // note the 'ref', which is
        {                                                     // important when assigning
            left = right;                                     // to value types!
        }
    }


    /// <summary>
    ///   A generic helper class to do common
    ///   <see cref = "System.Delegate">Delegate</see> operations.
    ///   
    ///   This ended up being necessary in order to get .NET 3.5 properly compiling, casting, and executing the created delegates.
    ///   Expression.Assign is not available on .NET 3.5
    ///   
    ///   Source: http://codereview.stackexchange.com/questions/1070/generic-advanced-delegate-createdelegate-using-expression-trees
    /// </summary>
    /// <author>Steven Jeuris</author>
    public static class DelegateHelper
    {
        /// <summary>
        ///   The name of the Invoke method of a Delegate.
        /// </summary>
        const string InvokeMethod = "Invoke";

        /// <summary>
        ///   Get method info for a specified delegate type.
        /// </summary>
        /// <param name = "delegateType">The delegate type to get info for.</param>
        /// <returns>The method info for the given delegate type.</returns>
        public static MethodInfo MethodInfoFromDelegateType(Type delegateType)
        {
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Given type should be a delegate.");
            }

            return delegateType.GetMethod(InvokeMethod);
        }

        /// <summary>
        ///   Creates a delegate of a specified type that represents the specified
        ///   static or instance method, with the specified first argument.
        ///   Conversions are done when possible.
        /// </summary>
        /// <typeparam name = "T">The type for the delegate.</typeparam>
        /// <param name = "firstArgument">
        ///   The object to which the delegate is bound,
        ///   or null to treat method as static
        /// </param>
        /// <param name = "method">
        ///   The MethodInfo describing the static or
        ///   instance method the delegate is to represent.
        /// </param>
        public static T CreateCompatibleDelegate<T>(
           object firstArgument,
           MethodInfo method)
        {
            MethodInfo delegateInfo = MethodInfoFromDelegateType(typeof(T));

            ParameterInfo[] methodParameters = method.GetParameters();
            ParameterInfo[] delegateParameters = delegateInfo.GetParameters();

            // Convert the arguments from the delegate argument type
            // to the method argument type when necessary.
            ParameterExpression[] arguments =
                (from delegateParameter in delegateParameters
                 select Expression.Parameter(delegateParameter.ParameterType, delegateParameter.Name))
                 .ToArray();
            Expression[] convertedArguments =
                new Expression[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; ++i)
            {
                Type methodType = methodParameters[i].ParameterType;
                Type delegateType = delegateParameters[i].ParameterType;
                if (methodType != delegateType)
                {
                    convertedArguments[i] =
                        Expression.Convert(arguments[i], methodType);
                }
                else
                {
                    convertedArguments[i] = arguments[i];
                }
            }

            // Create method call.
            ConstantExpression instance = firstArgument == null
                ? null
                : Expression.Constant(firstArgument);
            MethodCallExpression methodCall = Expression.Call(
                instance,
                method,
                convertedArguments
                );

            // Convert return type when necessary.
            Expression convertedMethodCall =
                delegateInfo.ReturnType == method.ReturnType
                    ? (Expression)methodCall
                    : Expression.Convert(methodCall, delegateInfo.ReturnType);

            return Expression.Lambda<T>(
                convertedMethodCall,
                arguments
                ).Compile();
        }
    }
}