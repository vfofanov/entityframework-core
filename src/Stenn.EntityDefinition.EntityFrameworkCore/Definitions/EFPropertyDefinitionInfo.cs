using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public class EFPropertyDefinitionInfo<T> : DefinitionInfo<T>, IEFPropertyDefinitionInfo<T>
    {
        private readonly Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, T?> _extract;

        /// <inheritdoc />
        public EFPropertyDefinitionInfo(string name, Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        public DefinitionInfo<T> Info => this;

        /// <inheritdoc />
        public T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context)
        {
            return _extract(property, propertyInfo, context);
        }
    }
}