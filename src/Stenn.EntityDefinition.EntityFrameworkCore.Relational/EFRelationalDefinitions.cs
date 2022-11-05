using Microsoft.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityFrameworkCore.Relational;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Relational
{
    /// <summary>
    /// Common Entity Framework model definitions
    /// </summary>
    public static class EFRelationalDefinitions
    {
        public static class Entities
        {
            public static readonly IEFEntityDefinition<string> DbName =
                new EFEntityDefinition<string>("DbName", (type, _) => type.GetTableName() ?? type.GetViewName());

            public static readonly IEFEntityDefinition<bool> IsTable =
                new EFEntityDefinition<bool>("IsTable", (type, _) => type.IsTable());

            public static readonly IEFEntityDefinition<bool> IsView =
                new EFEntityDefinition<bool>("IsView", (type, _) => type.IsView());
        }

        public static class Properties
        {
            public static readonly IEFPropertyDefinition<string> ColumnName =
                new EFScalarPropertyDefinition<string>("ColumnName", (property, _, _) => property.GetFinalColumnName());

            public static readonly IEFPropertyDefinition<string> ColumnType =
                new EFScalarPropertyDefinition<string>("ColumnType", (property, _, _) => property.GetColumnType());

            /// <summary>
            ///     <para>
            ///         Checks whether the column will be nullable in the database.
            ///     </para>
            ///     <para>
            ///         This depends on the property itself and also how it is mapped. For example,
            ///         derived non-nullable properties in a TPH type hierarchy will be mapped to nullable columns.
            ///         As well as properties on optional types sharing the same table.
            ///     </para>
            /// </summary>
            public static readonly IEFPropertyDefinition<bool> IsColumnNullable =
                new EFScalarPropertyDefinition<bool>("IsColumnNullable", (property, _, _) => property.IsColumnNullable());
        }
    }
}