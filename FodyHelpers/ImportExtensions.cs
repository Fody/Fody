using System;
using System.Linq;
using System.Linq.Expressions;

using Mono.Cecil;

namespace Fody
{
    public static class ImportExtensions
    {
        public static MethodReference ImportMethod<TResult>(this BaseModuleWeaver weaver, Expression<Func<TResult>> expression)
        {
            GetMethodInfo(expression, out var methodName, out var declaringTypeName, out var argumentTypeNames);

            var typeDefinition = weaver.FindType(declaringTypeName);

            try
            {
                var method = typeDefinition.Methods
                    .Single(m => (m.Name == methodName) && m.Parameters.Select(p => p.ParameterType.Name).SequenceEqual(argumentTypeNames));

                return weaver.ModuleDefinition.ImportReference(method);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Method {methodName} does not exist on type {declaringTypeName}", ex);
            }
        }

        public static MethodReference TryImportMethod<TResult>(this BaseModuleWeaver weaver, Expression<Func<TResult>> expression)
        {
            GetMethodInfo(expression, out var methodName, out var declaringTypeName, out var argumentTypeNames);

            if (!weaver.TryFindType(declaringTypeName, out var typeDefinition))
                return null;

            var method = typeDefinition.Methods
                .FirstOrDefault(m => m.Name == methodName && m.Parameters.Select(p => p.ParameterType.Name).SequenceEqual(argumentTypeNames));

            if (method == null)
                return null;

            return weaver.ModuleDefinition.ImportReference(method);
        }

        public static TypeReference ImportType<T>(this BaseModuleWeaver weaver)
        {
            return weaver.ModuleDefinition.ImportReference(weaver.FindType(typeof(T).Name));
        }

        public static TypeReference TryImportType<T>(this BaseModuleWeaver weaver)
        {
            if (!weaver.TryFindType(typeof(T).Name, out var typeDefinition))
                return null;

            return weaver.ModuleDefinition.ImportReference(typeDefinition);
        }

        private static void GetMethodInfo<TResult>(Expression<Func<TResult>> expression, out string methodName, out string declaringTypeName, out string[] argumentTypeNames)
        {
            if (!(expression.Body is MethodCallExpression methodCall))
                throw new ArgumentException("Only method call expression is supported.", nameof(expression));

            methodName = methodCall.Method.Name;
            declaringTypeName = methodCall.Method.DeclaringType.Name;
            argumentTypeNames = methodCall.Arguments.Select(a => a.Type.Name).ToArray();
        }
    }
}
