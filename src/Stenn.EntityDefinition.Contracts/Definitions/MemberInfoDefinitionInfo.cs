using System;
using System.Reflection;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public abstract class MemberInfoDefinitionInfo<T> : DefinitionInfo<T>
    {
        /// <inheritdoc />
        public MemberInfoDefinitionInfo(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {

        }
        public abstract T? Extract(MemberInfo? member, IDefinitionExtractContext context);

    }
}