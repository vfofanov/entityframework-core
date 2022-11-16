using System;
using System.Reflection;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public sealed class PropertyClrTypeDefinition : MemberInfoDefinition<TypeInfo>
    {
        /// <inheritdoc />
        public PropertyClrTypeDefinition(DefinitionInfo<TypeInfo> info) 
            : base(info)
        {
        }

        /// <inheritdoc />
        public PropertyClrTypeDefinition(string name) 
            : base(name, ConvertToString)
        {
        }

        private static string ConvertToString(TypeInfo type)
        {
            return type.HumanizeName();
        }

        /// <inheritdoc />
        public override TypeInfo? Extract(MemberInfo? member, TypeInfo? parentValue, DefinitionContext context)
        {
            if (member is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType.GetTypeInfo();
            }
            return default;
        }
    }
}