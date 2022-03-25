using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    public static class EnumTableExtinsions
    {
        public static IEnumerable<ModelEnumTable> ExtractEnumTables(this IModel model)
        {
            foreach (var group in model.GetEntityTypes().SelectMany(e => e.GetProperties())
                         .Distinct() // For abstract entity and inheritors: Proprty will appears count of inheritors
                         .Where(p => p.ClrType.IsEnum).GroupBy(p => p.ClrType))
            {
                var table = EnumTable.FromEnum(group.Key);
                yield return new ModelEnumTable(table, group.ToArray());
            }
        }

        public static string GetTableName(this IProperty prop)
        {
            return prop.DeclaringEntityType.GetTableName() ?? prop.DeclaringEntityType.Name;
        }
    }
}