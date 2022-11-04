using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFPropertyDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context);
    }

    public interface IEFPropertyDefinitionInfo<T> : IEFPropertyDefinitionInfo
    {
        /// <inheritdoc />
        DefinitionInfo IEFPropertyDefinitionInfo.Info => Info;

        /// <inheritdoc />
        object? IEFPropertyDefinitionInfo.Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context)
        {
            return Extract(property, propertyInfo, context);
        }

        new DefinitionInfo<T> Info { get; }
        new T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context);
    }
}