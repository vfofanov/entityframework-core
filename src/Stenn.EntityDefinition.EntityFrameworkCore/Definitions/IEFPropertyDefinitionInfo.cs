using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFPropertyDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, DefinitionContext context);
    }

    public interface IEFPropertyDefinition<T> : IEFPropertyDefinitionInfo
    {
        /// <inheritdoc />
        DefinitionInfo IEFPropertyDefinitionInfo.Info => Info;

        /// <inheritdoc />
        object? IEFPropertyDefinitionInfo.Extract(IPropertyBase? property, PropertyInfo? propertyInfo, DefinitionContext context)
        {
            return Extract(property, propertyInfo, context);
        }

        new DefinitionInfo<T> Info { get; }
        new T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, DefinitionContext context);
    }
}