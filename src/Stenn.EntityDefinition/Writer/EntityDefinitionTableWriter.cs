using System;
using System.Collections.Generic;
using System.Linq;
using Stenn.EntityDefinition.Contracts.Table;

namespace Stenn.EntityDefinition.Writer
{
    public sealed class EntityDefinitionTableWriter : IEntityDefinitionTableWriter
    {
        private EntityDefinitionTable? _table;

        /// <inheritdoc />
        public void SetColumns(IReadOnlyList<EntityDefinitionWriterColumn> columns)
        {
            var tableColumns = columns.Select(c => new EntityDefinitionTableColumn(
                c.Column.Info,
                c.WriterColumn.Name,
                c.Column.ColumnType,
                c.ConvertToString)).ToArray();
            
            _table = new EntityDefinitionTable(tableColumns);
        }

        /// <inheritdoc />
        public void WriteRow(object?[] values)
        {
            if (_table is null)
            {
                throw new ApplicationException("Call SetColumns first");
            }
            _table.AddRow(values);
        }

        public EntityDefinitionTable ToTable()
        {
            if (_table is null)
            {
                throw new ApplicationException("Call SetColumns first");
            }
            var ret = _table;
            _table = null;
            return ret;
        }
    }
}