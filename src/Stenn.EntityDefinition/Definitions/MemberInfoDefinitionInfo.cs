using System;
using System.Reflection;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Definitions
{
    public abstract class MemberInfoDefinitionInfo<T> : DefinitionInfo<T>,IMemberInfoDefinitionInfo
    {
        /// <inheritdoc />
        public MemberInfoDefinitionInfo(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {

        }

        public abstract T? Extract(MemberInfo? member, IDefinitionExtractContext context);

        /// <inheritdoc />
        DefinitionInfo IMemberInfoDefinitionInfo.Info => this;

        /// <inheritdoc />
        object? IMemberInfoDefinitionInfo.Extract(MemberInfo? member, IDefinitionExtractContext context)
        {
            return Extract(member, context);
        }
    }
}