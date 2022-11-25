using System;
using Stenn.Shared.Tables;

namespace Stenn.EntityDefinition.Writer
{
    public record EntityDefinitionWriterColumn(TableWriterColumn WriterColumn, IDefinitionColumn Column, Func<object?, string?> ConvertToString);
}