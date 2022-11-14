using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Writer;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public class EntityFrameworkCoreDefinitionOptions : IEntityFrameworkCoreDefinitionOptions
    {
        public EntityFrameworkCoreDefinitionReaderOptions ReaderOptions { get; } = new();
        public DefinitionWriterOptions WriterOptions { get; } = new();

        
        /// <inheritdoc />
        public void AddCommonConvert<T>(Func<T?, string?> convertToString)
        {
            WriterOptions.AddCommonConvert(convertToString);
        }

        EntityFrameworkDefinitionReaderOptions IEntityFrameworkCoreDefinitionOptions.ReaderOptions
        {
            get => ReaderOptions.ReaderOptions;
            set => ReaderOptions.ReaderOptions = value;
        }

        void IEntityFrameworkCoreDefinitionOptions.SetEntitiesFilter(Func<IEntityType, bool>? filter)
        {
            ReaderOptions.SetEntitiesFilter(filter);
        }

        void IEntityFrameworkCoreDefinitionOptions.SetPropertiesFilter(Func<IEntityType, IPropertyBase, bool>? filter)
        {
            ReaderOptions.SetPropertiesFilter(filter);
        }

        public void AddEntityColumn<T>(EFEntityDefinition<T> definition, string? columnName, Func<T?, string?>? convertToString)
        {
            ReaderOptions.AddEntityDefinition(definition);
            WriterOptions.AddEntityColumn(definition.Info, columnName, convertToString);
        }

        public void AddPropertyColumn<T>(EFPropertyDefinition<T> definition, string? columnName, Func<T?, string?>? convertToString)
        {
            ReaderOptions.AddPropertyDefinition(definition);
            WriterOptions.AddPropertyColumn(definition.Info, columnName, convertToString);
        }
    }
}