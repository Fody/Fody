using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Fody
{
    public abstract partial class BaseModuleWeaver
    {
        static XElement Empty = new XElement("Empty");

        /// <summary>
        /// The full element XML from FodyWeavers.xml.
        /// </summary>
        public XElement Config { get; set; } = Empty;

        static ParameterExpression targetParameter = Expression.Parameter(typeof(BaseModuleWeaver));
        static ParameterExpression boolParameter = Expression.Parameter(typeof(bool));

        ConcurrentDictionary<Tuple<Type,string>, Action<BaseModuleWeaver, bool>> boolSetters = new ConcurrentDictionary<Tuple<Type,string>, Action<BaseModuleWeaver, bool>>();

        /// <summary>
        /// Assign a field from <see cref="Config"/>.
        /// </summary>
        protected void AssignFromConfig(Expression<Func<bool>> field)
        {
            Guard.AgainstNull(nameof(field), field);
            var memberExpression = (MemberExpression) field.Body;
            var compile = field.Compile();

            var value = Config.ReadBool(memberExpression.Member.Name, compile());
            var assignExp = Expression.Assign(memberExpression, boolParameter);

            var setterExpression = Expression.Lambda<Action<BaseModuleWeaver, bool>>(assignExp, targetParameter, boolParameter);
            var setter = setterExpression.Compile();

            setter(this, value);

            Debug.WriteLine(value);
        }
    }
}