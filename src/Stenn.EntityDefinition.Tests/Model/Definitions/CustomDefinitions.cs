using System.Reflection;
using Stenn.EntityDefinition.Definitions;

namespace Stenn.EntityDefinition.Tests.Model.Definitions
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