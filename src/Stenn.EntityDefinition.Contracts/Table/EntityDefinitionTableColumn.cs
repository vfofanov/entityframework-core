using System;
using System.Diagnostics;

namespace Stenn.EntityDefinition.Contracts.Table
{
    [DebuggerDisplay("Info:{Info.Name}, Type:{ColumnType}, Name:{ColumnName}")]
    public record EntityDefinitionTableColumn(DefinitionInfo Info, string ColumnName,
        DefinitionColumnType ColumnType, Func<object?, string?> ConvertToStringFunc);
}