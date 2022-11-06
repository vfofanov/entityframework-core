using System;
using System.Collections.Generic;
using System.Linq;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Writer
{
    public sealed class EntityDefinitionWriter
    {
        private readonly DefinitionWriterOptions _options;

        public EntityDefinitionWriter(DefinitionWriterOptions options)
        {
            _options = options;
        }

        private IEnumerable<EntityDefinitionWriterColumn> GetColumns(IDefinitionMap map)
        {
            foreach (var column in _options.Columns)
            {
                var columnName = column.ColumnName ?? column.Info.Name;
                switch (column.ColumnType)
                {
                    case DefinitionColumnType.Entity:
                        if (!map.EntityDefinitions.Contains(column.Info))
                        {
                            continue;
                        }
                        break;
                    case DefinitionColumnType.Property:
                        if (!map.PropertyDefinitions.Contains(column.Info))
                        {
                            continue;
                        }
                        if (column.ColumnName is null && map.EntityDefinitions.Contains(column.Info))
                        {
                            columnName = $"P:{columnName}";
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return new EntityDefinitionWriterColumn(new TableWriterColumn(columnName, column.Info.ValueType), column, GetConvertToStringFunc(column));
            }
        }

        private Func<object?, string?> GetConvertToStringFunc(IDefinitionColumn column)
        {
            return column.ConvertToStringFunc ??
                   _options.CommonConverts.FirstOrDefault(c => c.Type == column.Info.ValueType)?.ConvertToString
                   ?? column.Info.ConvertToString;
        }


        public void Write(IDefinitionMap map, ITableWriter<object?> writer)
        {
            var columns = GetColumns(map).ToList();
            Write(columns, map, writer, (val, _) => val);
        }

        public void Write(IDefinitionMap map, ITableWriter<string?> writer)
        {
            var columns = GetColumns(map).ToList();
            Write(columns, map, writer, (val, c) => c.ConvertToString(val));
        }

        private static void Write<T>(IReadOnlyList<EntityDefinitionWriterColumn> columns, IDefinitionMap map, ITableWriter<T> writer,
            Func<object?, EntityDefinitionWriterColumn, T> convert)
        {
            writer.SetColumns(columns.Select(c => c.WriterColumn).ToArray());
            if (columns.All(c => c.Column.ColumnType != DefinitionColumnType.Property))
            {
                foreach (var entity in map.Entities)
                {
                    var row = new T[columns.Count];
                    for (var i = 0; i < columns.Count; i++)
                    {
                        var column = columns[i];
                        var val = convert(entity.Values[column.Column.Info], column);
                        row[i] = val;
                    }
                    writer.WriteRow(row);
                }
            }
            else
            {
                foreach (var entity in map.Entities)
                {
                    foreach (var property in entity.Properties)
                    {
                        var row = new T[columns.Count];
                        for (var i = 0; i < columns.Count; i++)
                        {
                            var column = columns[i];
                            var values = column.Column.ColumnType switch
                            {
                                DefinitionColumnType.Entity => entity.Values,
                                DefinitionColumnType.Property => property.Values,
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            var val = convert(values[column.Column.Info], column);
                            row[i] = val;
                        }
                        writer.WriteRow(row);
                    }
                }
            }
        }
    }

    public record EntityDefinitionWriterColumn(TableWriterColumn WriterColumn, IDefinitionColumn Column, Func<object?, string?> ConvertToString);

    public record TableWriterColumn(string Name, Type ColumnType);

    public interface ITableWriter<in T>
    {
        void SetColumns(params TableWriterColumn[] columns);
        void WriteRow(T?[] values);
    }
}