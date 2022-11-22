using System;

namespace Stenn.EntityDefinition.Contracts.Table
{
    public sealed class EntityDefinitionTableRow
    {
        private readonly EntityDefinitionTable _table;
        private readonly object?[] _row;

        public EntityDefinitionTableRow(EntityDefinitionTable table, object?[] row)
        {
            _table = table;
            _row = row;
        }

        public object? this[int index] => _row[index];

        public T? Get<T>(DefinitionInfo<T> info, DefinitionColumnType type)
        {
            var index = _table.GetIndexOfColumn(info, type);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(info), "Can't find column");
            }
            return (T?)_row[index];
        }
            
        public string? GetString(DefinitionInfo info, DefinitionColumnType type)
        {
            var index = _table.GetIndexOfColumn(info, type);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(info), "Can't find column");
            }
            var column = _table.Columns[index];
                
            return column.ConvertToStringFunc(_row[index]);
        }
    }
}