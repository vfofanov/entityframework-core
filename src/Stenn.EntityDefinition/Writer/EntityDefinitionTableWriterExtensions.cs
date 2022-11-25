using System;
using Stenn.Shared.Tables;

namespace Stenn.EntityDefinition.Writer
{
    public static class EntityDefinitionTableWriterExtensions
    {
        public static IEntityDefinitionTableWriter ToDefinitionTableWriter<T>(this ITableWriter<T> writer,
            Func<object?,EntityDefinitionWriterColumn,T> convert)
        {
            return new EntityDefinitionTableWriterWrapper<T>(writer, convert);
        }
    }
}