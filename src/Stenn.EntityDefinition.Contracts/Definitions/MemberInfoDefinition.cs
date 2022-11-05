using System;
using System.Reflection;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public abstract class MemberInfoDefinition<T> : Definition<T>
    {
        /// <inheritdoc />
        protected MemberInfoDefinition(DefinitionInfo<T> info) : base(info)
        {
        }

        /// <inheritdoc />
        protected MemberInfoDefinition(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {

        }
        public abstract T? Extract(MemberInfo? member, DefinitionContext context);

    }

    
}