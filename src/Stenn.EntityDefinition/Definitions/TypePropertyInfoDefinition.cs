using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.Definitions
{
    internal sealed class TypePropertyFieldInfoDefinition : MemberInfoDefinition<string> 
    {
        /// <inheritdoc />
        public TypePropertyFieldInfoDefinition() 
            : base("ClrType")
        {
        }

        /// <inheritdoc />
        public override string? Extract(MemberInfo? member, string? parentValue, DefinitionContext context)
        {
            Type? clrType = null;
            if (member is PropertyInfo p)
            {
                clrType = p.PropertyType;
            }
            if (member is FieldInfo f)
            {
                clrType = f.FieldType;
            }
            
            return member?.Name;
        }

        
    }
}