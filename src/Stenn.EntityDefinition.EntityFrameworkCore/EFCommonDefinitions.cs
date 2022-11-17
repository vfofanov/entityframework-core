using System;
using System.Reflection;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    /// <summary>
    /// Common Entity Framework model definitions
    /// </summary>
    public static class EFCommonDefinitions
    {
        public static class Entities
        {
            public static readonly EFEntityDefinition<string> Name = CommonDefinitions.Name.ToEntity();
            public static readonly EFEntityDefinition<string> Remark = CommonDefinitions.Remark.ToEntity();

            public static readonly EFEntityDefinition<bool> IsObsolete = CommonDefinitions.IsObsolete.ToEntity();
            public static readonly EFEntityDefinition<string> ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToEntity();
            
            /// <summary>
            /// Base type for entity
            /// </summary>
            public static readonly EFEntityDefinition<string?> BaseEntityName = new("BaseEntityName", 
                (entity, _, _) => entity.BaseType?.ClrType?.Name);
            
            /// <summary>
            /// Gets definition for <see cref="CommonDefinitions.XmlDescription"/>
            /// </summary>
            /// <returns></returns>
            public static EFEntityDefinition<string> GetXmlDescription(Func<Assembly, XPathDocument?>? getCommentDoc = null) =>
                CommonDefinitions.GetXmlDescription(getCommentDoc).ToEntity();
        }

        public static class Properties
        {
            public static readonly EFPropertyDefinition<string> Name = new("Name", (_, _, _, _, row, _) => row.Name);
            public static readonly EFPropertyDefinition<string> Remark = CommonDefinitions.Remark.ToProperty();

            public static readonly EFPropertyDefinition<bool> IsObsolete = CommonDefinitions.IsObsolete.ToProperty();
            public static readonly EFPropertyDefinition<string> ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToProperty();

            public static readonly EFPropertyDefinition<bool> IsShadow = new("IsShadow", (property, _, _, _, _, _) => property?.IsShadowProperty() ?? false);

            public static readonly EFPropertyDefinition<bool> IsNavigation = new("IsNavigation", (property, _, _, _, _, _) => property is INavigation);

            /// <summary>
            ///     Gets a value indicating whether this property can contain <see langword="null" />.
            /// </summary>
            public static readonly EFPropertyDefinition<bool> IsNullable =
                new EFScalarPropertyDefinition<bool>("IsNullable", (property, _, _, _, _, _) => property.IsNullable);
            
            /// <summary>
            ///     Gets a clr type of property
            /// </summary>
            public static readonly EFPropertyDefinition<TypeInfo> ClrType = CommonDefinitions.PropertyClrType.ToProperty();

            /// <summary>
            /// Gets definition for <see cref="CommonDefinitions.XmlDescription"/>
            /// </summary>
            /// <returns></returns>
            public static EFPropertyDefinition<string> GetXmlDescription(Func<Assembly, XPathDocument?>? getCommentDoc = null) =>
                CommonDefinitions.GetXmlDescription(getCommentDoc).ToProperty();
        }
    }
}