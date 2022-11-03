using System;
using System.Reflection;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition
{
    public abstract class MemberInfoDefinitionInfo<T, TContext> : DefinitionInfo<T>
        where TContext : IDefinitionExtractContext
    {
        /// <inheritdoc />
        public MemberInfoDefinitionInfo(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {

        }

        public abstract T Extract(MemberInfo member, TContext context);
    }
}