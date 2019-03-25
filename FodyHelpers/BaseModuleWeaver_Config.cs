using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using SetBool = System.Action<Fody.BaseModuleWeaver, bool>;
using GetBool = System.Func<Fody.BaseModuleWeaver, bool>;
using SetString = System.Action<Fody.BaseModuleWeaver, string>;
using GetString = System.Func<Fody.BaseModuleWeaver, string>;

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
        static ConcurrentDictionary<(Type, string member), (GetBool getter, SetBool setter)> boolAccessors = new ConcurrentDictionary<(Type, string member), (GetBool, SetBool)>();
        static ConcurrentDictionary<(Type, string member), (GetString getter, SetString setter)> stringAccessors = new ConcurrentDictionary<(Type, string member), (GetString, SetString)>();

        /// <summary>
        /// Assign a field from <see cref="Config"/>.
        /// </summary>
        protected void AssignFromConfig(Expression<Func<bool>> field)
        {
            Guard.AgainstNull(nameof(field), field);
            Assign(boolAccessors, field, ConfigReader.ReadBool);
        }

        /// <summary>
        /// Assign a field from <see cref="Config"/>.
        /// </summary>
        protected void AssignFromConfig(Expression<Func<string>> field)
        {
            Guard.AgainstNull(nameof(field), field);
            Assign(stringAccessors, field, ConfigReader.ReadString);
        }

        void Assign<TField>(
            ConcurrentDictionary<(Type, string member), (Func<BaseModuleWeaver, TField> getter, Action<BaseModuleWeaver, TField> setter)> dictionary,
            Expression<Func<TField>> field,
            Func<XElement, string, TField, TField> func)
        {
            var memberExpression = (MemberExpression) field.Body;
            var name = memberExpression.Member.Name;
            var accessor = dictionary.GetOrAdd(
                key: (GetType(), name),
                valueFactory: x => BuildAccessor<Func<BaseModuleWeaver, TField>, Action<BaseModuleWeaver, TField>>(name));

            var value = accessor.getter(this);
            value = func(Config, name, value);
            accessor.setter(this, value);
        }

        (TGet getter, TSet setter) BuildAccessor<TGet, TSet>(string name)
        {
            var first = typeof(TGet).GetGenericArguments().Last();
            var valueParameter = Expression.Parameter(first);
            var converted = Expression.Convert(targetParameter, GetType());

            var field = Expression.Field(converted, name);
            var getLambda = Expression.Lambda<TGet>(field, targetParameter);
            var assignExp = Expression.Assign(field, valueParameter);
            var setLambda = Expression.Lambda<TSet>(assignExp, targetParameter, valueParameter);
            return (getLambda.Compile(), setLambda.Compile());
        }
    }
}