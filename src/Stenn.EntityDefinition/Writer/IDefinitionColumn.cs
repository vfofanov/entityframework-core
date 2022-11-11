using System;
using Stenn.EntityDefinition.Contracts;

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