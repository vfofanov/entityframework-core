using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.EntityDefinition.Writer;
using Stenn.Shared.Tables;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public static class EntityFrameworkCoreDefinitionsExtensions
    {
        public static string GenerateCsv(this IModel model,
            Action<IEntityFrameworkCoreDefinitionOptions> readerOptionsInit,
            char delimiter = ',')
        {
            var options = new EntityFrameworkCoreDefinitionOptions();
            readerOptionsInit(options);

            return model.GenerateCsv(options, delimiter);
        }

        public static string GenerateCsv(this IModel model,
            EntityFrameworkCoreDefinitionOptions options,
            char delimiter = ',')
        {
            var reader = new EntityFrameworkCoreDefinitionReader(model, options.ReaderOptions);
            var map = reader.Read();

            var writer = new EntityDefinitionWriter(options.WriterOptions);
            var tableWriter = new CsvTableWriter(delimiter);

            writer.Write(map, tableWriter);
            return tableWriter.Build();
        }

        public static EntityDefinitionTable GenerateEntityDefinitionTable(this IModel model,
            Action<IEntityFrameworkCoreDefinitionOptions> readerOptionsInit)
        {
            var options = new EntityFrameworkCoreDefinitionOptions();
            readerOptionsInit(options);

            return model.GenerateEntityDefinitionTable(options);
        }

        public static EntityDefinitionTable GenerateEntityDefinitionTable(this IModel model,
            EntityFrameworkCoreDefinitionOptions options)
        {
            var reader = new EntityFrameworkCoreDefinitionReader(model, options.ReaderOptions);
            var map = reader.Read();

            var writer = new EntityDefinitionWriter(options.WriterOptions);

            var table = writer.Write(map);
            return table;
        }

        public static string Generate(this IModel model,
            EntityFrameworkCoreDefinitionOptions options, Func<DefinitionMap, string> generateFunc)
        {
            var reader = new EntityFrameworkCoreDefinitionReader(model, options.ReaderOptions);
            var map = reader.Read();
            return generateFunc(map);
        }
    }
}