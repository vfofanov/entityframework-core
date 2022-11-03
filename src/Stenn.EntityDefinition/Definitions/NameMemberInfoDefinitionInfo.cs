using System.Reflection;

namespace Stenn.EntityDefinition
{
    internal sealed class NameMemberInfoDefinitionInfo : MemberInfoDefinitionInfo<string, IDefinitionExtractContext> 
    {
        /// <inheritdoc />
        public NameMemberInfoDefinitionInfo() 
            : base("Name")
        {
        }

        /// <inheritdoc />
        public override string Extract(MemberInfo member, IDefinitionExtractContext context)
        {
            return member.Name;
        }
    }
}