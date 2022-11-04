using System.Reflection;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.Tests.Model.Definitions;

namespace Stenn.EntityDefinition.Model.Definitions
{
    public static class CustomDefinitions
    {
        /// <summary>
        /// Name definition based on <see cref="MemberInfo.Name"/>
        /// </summary>
        public static MemberInfoDefinitionInfo<Domain> Domain =
            new AttributeDefinitionInfo<Domain, DefinitionDomainAttribute>("Domain");
    }
}