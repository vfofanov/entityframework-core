using Microsoft.EntityFrameworkCore;
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
            public static readonly IEFEntityDefinitionInfo<string> Name = CommonDefinitions.Name.ToEntity();
            public static readonly IEFEntityDefinitionInfo<string> Remark = CommonDefinitions.Remark.ToEntity();

            public static readonly IEFEntityDefinitionInfo<bool> IsObsolete = CommonDefinitions.IsObsolete.ToEntity();
            public static readonly IEFEntityDefinitionInfo<string> ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToEntity();

            
        }

        public static class Properties
        {
            public static readonly IEFPropertyDefinitionInfo<string> Name = CommonDefinitions.Name.ToProperty();
            public static readonly IEFPropertyDefinitionInfo<string> Remark = CommonDefinitions.Remark.ToProperty();

            public static readonly IEFPropertyDefinitionInfo<bool> IsObsolete = CommonDefinitions.IsObsolete.ToProperty();
            public static readonly IEFPropertyDefinitionInfo<string> ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToProperty();

            public static readonly IEFPropertyDefinitionInfo<bool> IsShadow =
                new EFPropertyDefinitionInfo<bool>("IsShadow", (property, _, _) => property?.IsShadowProperty() ?? false);

            /// <summary>
            ///     Gets a value indicating whether this property can contain <see langword="null" />.
            /// </summary>
            public static readonly IEFPropertyDefinitionInfo<bool> IsNullable =
                new EFScalarPropertyDefinitionInfo<bool>("IsNullable", (property, _, _) => property.IsNullable);
        }
    }
}