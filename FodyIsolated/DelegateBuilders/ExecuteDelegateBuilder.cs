using System;
using System.Reflection;

public static class ExecuteDelegateBuilder
{


    public static Action<object> BuildExecuteDelegate(this Type weaverType)
    {
        var executeMethod = weaverType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public, null, new Type[] {}, null);
        if (executeMethod == null)
        {
            var message = string.Format("{0} must contain a public instance method named 'Execute'.", weaverType.FullName);
            throw new WeavingException(message);
        }

        return o =>
            {
                try
                {
                    executeMethod.Invoke(o, null);
                }
                catch (TargetInvocationException invocationException)
                {
                    throw invocationException.InnerException;
                }
            };
    }

}