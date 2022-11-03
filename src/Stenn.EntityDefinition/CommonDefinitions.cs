using System.Reflection;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Definitions;

namespace Stenn.EntityDefinition
{
    /// <summary>
    /// Common model definitions
    /// </summary>
    public static class CommonDefinitions
    {
        /// <summary>
        /// Name definition based on <see cref="MemberInfo.Name"/>
        /// </summary>
        public static MemberInfoDefinitionInfo<string> Name = new NameMemberInfoDefinitionInfo();

        /// <summary>
        /// Remark based on <see cref="DefinitionRemarkAttribute"/>
        /// </summary>
        public static AttributeDefinitionInfo<string, DefinitionRemarkAttribute> Remark = new("Remark");
    }
}