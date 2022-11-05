using System.Reflection;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.Model.Definitions
{
    public static class CustomDefinitions
    {
        /// <summary>
        /// Name definition based on <see cref="MemberInfo.Name"/>
        /// </summary>
        public static MemberInfoDefinition<Domain> Domain =
            new AttributeDefinition<Domain, DefinitionDomainAttribute>("Domain");
    }
}