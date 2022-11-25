using System.Collections.Generic;

namespace Stenn.EntityDefinition.Writer
{
    public interface IEntityDefinitionTableWriter
    {
        void SetColumns(IReadOnlyList<EntityDefinitionWriterColumn> columns);
        void WriteRow(object?[] values);
    }
}