using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Table;

namespace Stenn.EntityDefinition.Writer
{
    public interface IDefinitionColumn
    {
        DefinitionInfo Info { get; }
        string? ColumnName { get; }
        DefinitionColumnType ColumnType { get; }
        Func<object?, string?>? ConvertToStringFunc { get; }
    }
}