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
            public static IEFEntityDefinitionInfo Name = CommonDefinitions.Name.ToEntity();
            public static IEFEntityDefinitionInfo Remark = CommonDefinitions.Remark.ToEntity();
        }

        public static class Properties
        {
            public static IEFPropertyDefinitionInfo Name = CommonDefinitions.Name.ToProperty();
            public static IEFPropertyDefinitionInfo Remark = CommonDefinitions.Remark.ToProperty();
        }
    }
}