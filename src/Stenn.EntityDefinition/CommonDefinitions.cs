using System;
using System.Reflection;
using System.Xml.XPath;
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
        public static MemberInfoDefinition<string> Name = new NameMemberInfoDefinition();

        /// <summary>
        /// Remark based on <see cref="DefinitionRemarkAttribute"/>
        /// </summary>
        public static MemberInfoDefinition<string> Remark = new AttributeDefinition<string, DefinitionRemarkAttribute>("Remark");

        /// <summary>
        /// IsObsolete or not based on <see cref="ObsoleteAttribute"/>
        /// </summary>
        public static MemberInfoDefinition<bool> IsObsolete = new CustomAttributeDefinition<bool, ObsoleteAttribute>("IsObsolete", (_, _) => true);

        /// <summary>
        /// Obsolete message or not based on <see cref="ObsoleteAttribute"/>
        /// </summary>
        public static MemberInfoDefinition<string> ObsoleteMessage =
            new CustomAttributeDefinition<string, ObsoleteAttribute>("IsObsolete", (attr, _) => attr.Message);

        /// <summary>
        /// Description from xml documentation
        /// </summary>
        /// <returns></returns>
        public static DefinitionInfo<string> XmlDescription = new("Description");

        /// <summary>
        /// Gets definition for <see cref="CommonDefinitions.XmlDescription"/>
        /// </summary>
        /// <returns></returns>
        public static MemberInfoDefinition<string> GetXmlDescription(Func<Assembly, XPathDocument?>? getCommentDoc = null)
            => new XmlDescriptionMemberInfoDefinition(XmlDescription, getCommentDoc);
    }
}