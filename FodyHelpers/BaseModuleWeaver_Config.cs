using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Xml.Linq;
using SetBool = System.Action<Fody.BaseModuleWeaver, bool>;
using GetBool = System.Func<Fody.BaseModuleWeaver, bool>;

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

        static ConcurrentDictionary<(Type, string member), (GetBool getter, SetBool setter)> boolAccessor = new ConcurrentDictionary<(Type, string member), (GetBool, SetBool)>();

        /// <summary>
        /// Assign a field from <see cref="Config"/>.
        /// </summary>
        protected void AssignFromConfig(Expression<Func<bool>> field)
        {
            Guard.AgainstNull(nameof(field), field);

            var memberExpression = (MemberExpression) field.Body;
            var name = memberExpression.Member.Name;

            var accessor = boolAccessor.GetOrAdd((GetType(), name),
                x =>
                {
                    var getLambda = Expression.Lambda<GetBool>(memberExpression, targetParameter);

                    var assignExp = Expression.Assign(memberExpression, boolParameter);
                    var setLambda = Expression.Lambda<SetBool>(assignExp, targetParameter, boolParameter);
                    return (getLambda.Compile(), setLambda.Compile());
                });

            var value = accessor.getter(this);
            accessor.setter(this, value);
        }
    }
}