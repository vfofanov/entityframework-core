using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Writer;

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
            var csvWriter = new CsvTableWriter(delimiter);

            writer.Write(map, csvWriter);
            return csvWriter.Build();
        }
    }
}