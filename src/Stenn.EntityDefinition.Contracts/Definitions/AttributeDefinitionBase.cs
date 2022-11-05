using System;
using System.Reflection;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public abstract class AttributeDefinitionBase<T, TAttr> : MemberInfoDefinition<T> 
        where TAttr : Attribute
    {
        /// <inheritdoc />
        protected AttributeDefinitionBase(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
        }

        /// <inheritdoc />
        public override T? Extract(MemberInfo? member, DefinitionContext context)
        {
            TAttr? attr = null;
            if (member is { })
            {
                attr = member.GetCustomAttribute<TAttr>() ?? GetParentAttribute(member);
            }
            if (attr is { } a)
            {
                return GetValue(a, context);
            }
            return default;
        }

        private static TAttr? GetParentAttribute(MemberInfo member)
        {
            return member.MemberType == MemberTypes.TypeInfo ? member.Module.Assembly.GetCustomAttribute<TAttr>() : null;
        }
        
        protected abstract T? GetValue(TAttr attr, DefinitionContext context);
    }
}