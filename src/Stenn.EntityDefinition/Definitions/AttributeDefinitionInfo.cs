using System;
using System.Reflection;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Definitions
{
    public sealed class AttributeDefinitionInfo<T, TAttr> : MemberInfoDefinitionInfo<T, IDefinitionExtractContext>
        where TAttr : DefinitionAttribute
    {
        /// <inheritdoc />
        public AttributeDefinitionInfo(string name, Func<T, string>? convertToString = null) 
            : base(name, convertToString)
        {
        }

        /// <inheritdoc />
        public override T? Extract(MemberInfo member, IDefinitionExtractContext context)
        {
            var attr = member.GetCustomAttribute<TAttr>() ??
                       context.GetAssemblyAttribute<TAttr>(member);
            if (attr?.Value is { } v)
            {
                return (T?)v;
            }
            return default;
        }
    }
}