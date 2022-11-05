using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public class EFPropertyDefinition<T> : Definition<T>, IEFPropertyDefinition<T>
    {
        private readonly Func<IPropertyBase?, PropertyInfo?, DefinitionContext, T?> _extract;

        /// <inheritdoc />
        public EFPropertyDefinition(DefinitionInfo<T> info, Func<IPropertyBase?, PropertyInfo?, DefinitionContext, T?> extract)
            : base(info)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }


        /// <inheritdoc />
        public EFPropertyDefinition(string name, Func<IPropertyBase?, PropertyInfo?, DefinitionContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        public T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, DefinitionContext context)
        {
            return _extract(property, propertyInfo, context);
        }
    }
}