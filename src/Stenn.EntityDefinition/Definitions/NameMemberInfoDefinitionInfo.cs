using System.Reflection;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.Definitions
{
    internal sealed class NameMemberInfoDefinition : MemberInfoDefinition<string> 
    {
        /// <inheritdoc />
        public NameMemberInfoDefinition() 
            : base("Name")
        {
        }

        /// <inheritdoc />
        public override string? Extract(MemberInfo? member, DefinitionContext context)
        {
            return member?.Name;
        }
    }
}