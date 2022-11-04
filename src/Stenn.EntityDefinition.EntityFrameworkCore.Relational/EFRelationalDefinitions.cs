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
            public static readonly IEFEntityDefinitionInfo<string> DbName =
                new EFEntityDefinitionInfo<string>("DbName", (type, _) => type.GetTableName() ?? type.GetViewName());

            public static readonly IEFEntityDefinitionInfo<bool> IsTable =
                new EFEntityDefinitionInfo<bool>("IsTable", (type, _) => type.IsTable());

            public static readonly IEFEntityDefinitionInfo<bool> IsView =
                new EFEntityDefinitionInfo<bool>("IsView", (type, _) => type.IsView());
        }

        public static class Properties
        {
            public static readonly IEFPropertyDefinitionInfo<string> ColumnName =
                new EFScalarPropertyDefinitionInfo<string>("ColumnName", (property, _, _) => property.GetFinalColumnName());

            public static readonly IEFPropertyDefinitionInfo<string> ColumnType =
                new EFScalarPropertyDefinitionInfo<string>("ColumnType", (property, _, _) => property.GetColumnType());

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
            public static readonly IEFPropertyDefinitionInfo<bool> IsColumnNullable =
                new EFScalarPropertyDefinitionInfo<bool>("IsColumnNullable", (property, _, _) => property.IsColumnNullable());
        }
    }
}