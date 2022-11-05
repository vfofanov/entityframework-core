using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFPropertyDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, object? parentValue, DefinitionContext context);
    }

    public interface IEFPropertyDefinition<T> : IEFPropertyDefinitionInfo
    {
        /// <inheritdoc />
        DefinitionInfo IEFPropertyDefinitionInfo.Info => Info;

        /// <inheritdoc />
        object? IEFPropertyDefinitionInfo.Extract(IPropertyBase? property, PropertyInfo? propertyInfo, object? parentValue, DefinitionContext context)
        {
            return parentValue is null
                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                ? Extract(property, propertyInfo, default(T?), context)
                : Extract(property, propertyInfo, (T?)parentValue, context);
        }

        new DefinitionInfo<T> Info { get; }
        T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, T? parentValue, DefinitionContext context);
    }
}