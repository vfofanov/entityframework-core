using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public interface IEntityFrameworkCoreDefinitionOptions
    {
        void AddCommonConvert<T>(Func<T?, string?> convertToString);
        EntityFrameworkDefinitionReaderOptions ReaderOptions { get; set; }
        void SetEntitiesFilter(Func<IEntityType, bool>? filter);
        void SetPropertiesFilter(Func<IEntityType, IPropertyBase, bool>? filter);
        void AddEntityColumn<T>(EFEntityDefinition<T> definition, string? columnName = null, Func<T?, string?>? convertToString = null);
        void AddPropertyColumn<T>(EFPropertyDefinition<T> definition, string? columnName = null, Func<T?, string?>? convertToString = null);
    }
}