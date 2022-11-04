using System;
using System.Reflection;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
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
        public static MemberInfoDefinitionInfo<string> Remark = new AttributeDefinitionInfo<string, DefinitionRemarkAttribute>("Remark");

        /// <summary>
        /// IsObsolete or not based on <see cref="ObsoleteAttribute"/>
        /// </summary>
        public static MemberInfoDefinitionInfo<bool> IsObsolete = new CustomAttributeDefinitionInfo<bool, ObsoleteAttribute>("IsObsolete", _ => true);

        /// <summary>
        /// IsObsolete or not based on <see cref="ObsoleteAttribute"/>
        /// </summary>
        public static MemberInfoDefinitionInfo<string> ObsoleteMessage =
            new CustomAttributeDefinitionInfo<string, ObsoleteAttribute>("IsObsolete", attr => attr.Message);
    }
}