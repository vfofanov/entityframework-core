using System.Reflection;

namespace Stenn.EntityDefinition.Definitions
{
    internal sealed class NameMemberInfoDefinitionInfo : MemberInfoDefinitionInfo<string> 
    {
        /// <inheritdoc />
        public NameMemberInfoDefinitionInfo() 
            : base("Name")
        {
        }

        /// <inheritdoc />
        public override string? Extract(MemberInfo? member, IDefinitionExtractContext context)
        {
            return member?.Name;
        }
    }
}