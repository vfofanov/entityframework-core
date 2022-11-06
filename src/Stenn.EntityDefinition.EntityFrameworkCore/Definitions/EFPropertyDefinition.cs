using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public class EFPropertyDefinition<T> : Definition<T>, IEFPropertyDefinition
    {
        private readonly Func<IPropertyBase?, PropertyInfo?, T?, DefinitionContext, T?> _extract;

        /// <inheritdoc />
        public EFPropertyDefinition(DefinitionInfo<T> info, Func<IPropertyBase?, PropertyInfo?, T?, DefinitionContext, T?> extract)
            : base(info)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }


        /// <inheritdoc />
        public EFPropertyDefinition(string name, Func<IPropertyBase?, PropertyInfo?, T?, DefinitionContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        public T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, T? parentValue, DefinitionContext context)
        {
            return _extract(property, propertyInfo, parentValue, context);
        }

        /// <inheritdoc />
        public object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, object? parentValue, DefinitionContext context)
        {
            return parentValue is null
                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                ? Extract(property, propertyInfo, default(T?), context)
                : Extract(property, propertyInfo, (T?)parentValue, context);
        }
    }
}