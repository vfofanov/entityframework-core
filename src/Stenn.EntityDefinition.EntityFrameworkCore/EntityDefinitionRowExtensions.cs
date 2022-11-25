using Stenn.EntityDefinition.Contracts.Table;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public static class EntityDefinitionRowExtensions
    {
        public static T? Get<T>(this EntityDefinitionTableRow row, EFEntityDefinition<T> definition)
        {
            return row.Get(definition.Info, DefinitionColumnType.Entity);
        }

        public static string? GetString<T>(this EntityDefinitionTableRow row, EFEntityDefinition<T> definition)
        {
            return row.GetString(definition.Info, DefinitionColumnType.Entity);
        }

        public static T? Get<T>(this EntityDefinitionTableRow row, EFPropertyDefinition<T> definition)
        {
            return row.Get(definition.Info, DefinitionColumnType.Property);
        }

        public static string? GetString<T>(this EntityDefinitionTableRow row, EFPropertyDefinition<T> definition)
        {
            return row.GetString(definition.Info, DefinitionColumnType.Property);
        }
    }
}