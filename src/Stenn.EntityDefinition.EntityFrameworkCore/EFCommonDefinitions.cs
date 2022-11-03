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
            public static readonly IEFEntityDefinitionInfo Name = CommonDefinitions.Name.ToEntity();
            public static readonly IEFEntityDefinitionInfo Remark = CommonDefinitions.Remark.ToEntity();

            public static readonly IEFEntityDefinitionInfo IsObsolete = CommonDefinitions.IsObsolete.ToEntity();
            public static readonly IEFEntityDefinitionInfo ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToEntity();
        }

        public static class Properties
        {
            public static readonly IEFPropertyDefinitionInfo Name = CommonDefinitions.Name.ToProperty();
            public static readonly IEFPropertyDefinitionInfo Remark = CommonDefinitions.Remark.ToProperty();
            
            public static readonly IEFPropertyDefinitionInfo IsObsolete = CommonDefinitions.IsObsolete.ToProperty();
            public static readonly IEFPropertyDefinitionInfo ObsoleteMessage = CommonDefinitions.ObsoleteMessage.ToProperty();
        }
    }
}