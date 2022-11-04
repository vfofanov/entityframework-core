using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public static class EFDefinitionExtensions
    {
        public static IEFEntityDefinitionInfo<T> ToEntity<T>(this MemberInfoDefinitionInfo<T> info)
        {
            return new EFEntityDefinition<T>(info, (type, context) => info.Extract(type.ClrType, context));
        }

        public static IEFPropertyDefinitionInfo<T> ToProperty<T>(this MemberInfoDefinitionInfo<T> info)
        {
            return new EFPropertyDefinition<T>(info, (_, propertyInfo, context) => info.Extract(propertyInfo, context));
        }

        private sealed class EFEntityDefinition<T> : IEFEntityDefinitionInfo<T>
        {
            private readonly Func<IEntityType, IEFDefinitionExtractContext, T?> _extract;

            internal EFEntityDefinition(DefinitionInfo<T> info, Func<IEntityType, IEFDefinitionExtractContext, T?> extract)
            {
                _extract = extract;
                Info = info;
            }

            /// <inheritdoc />
            public DefinitionInfo<T> Info { get; }

            /// <inheritdoc />
            public T? Extract(IEntityType type, IEFDefinitionExtractContext context)
            {
                return _extract(type, context);
            }
        }

        private sealed class EFPropertyDefinition<T> : IEFPropertyDefinitionInfo<T>
        {
            private readonly Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, T?> _extract;

            internal EFPropertyDefinition(DefinitionInfo<T> info, Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, T?> extract)
            {
                _extract = extract;
                Info = info;
            }

            /// <inheritdoc />
            public DefinitionInfo<T> Info { get; }

            /// <inheritdoc />
            public T? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context)
            {
                return _extract(property, propertyInfo, context);
            }
        }
    }
}