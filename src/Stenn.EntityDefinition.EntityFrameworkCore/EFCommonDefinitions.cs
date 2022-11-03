using System.Reflection;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Definitions;

namespace Stenn.EntityDefinition
{
    /// <summary>
    /// Common Entity Framework model definitions
    /// </summary>
    public static class EFCommonDefinitions
    {
        public static class Entities
        {
            
        }
        
        public static class Properties
        {
            
        }

        /// <summary>
        /// Name definition based on <see cref="MemberInfo.Name"/>
        /// </summary>
        public static MemberInfoDefinitionInfo<string, IDefinitionExtractContext> Name = new NameMemberInfoDefinitionInfo();

        /// <summary>
        /// Remark based on <see cref="DefinitionRemarkAttribute"/>
        /// </summary>
        public static AttributeDefinitionInfo<string, DefinitionRemarkAttribute> Remark = new("Remark");
    }
}