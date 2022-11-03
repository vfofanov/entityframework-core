using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public static class EFDefinitionExtensions
    {
        public static IEFEntityDefinitionInfo ToEntity<T>(this MemberInfoDefinitionInfo<T> info)
        {
            return new EFEntityDefinition(info, (type, context) => info.Extract(type.ClrType, context));
        }

        public static IEFPropertyDefinitionInfo ToProperty<T>(this MemberInfoDefinitionInfo<T> info)
        {
            return new EFPropertyDefinition(info, (_, propertyInfo, context) => info.Extract(propertyInfo, context));
        }

        private sealed class EFEntityDefinition : IEFEntityDefinitionInfo
        {
            private readonly Func<IEntityType, IEFDefinitionExtractContext, object?> _extract;

            internal EFEntityDefinition(DefinitionInfo info, Func<IEntityType, IEFDefinitionExtractContext, object?> extract)
            {
                _extract = extract;
                Info = info;
            }

            /// <inheritdoc />
            public DefinitionInfo Info { get; }

            /// <inheritdoc />
            public object? Extract(IEntityType type, IEFDefinitionExtractContext context)
            {
                return _extract(type, context);
            }
        }

        private sealed class EFPropertyDefinition : IEFPropertyDefinitionInfo
        {
            private readonly Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, object?> _extract;

            internal EFPropertyDefinition(DefinitionInfo info, Func<IPropertyBase?, PropertyInfo?, IEFDefinitionExtractContext, object?> extract)
            {
                _extract = extract;
                Info = info;
            }

            /// <inheritdoc />
            public DefinitionInfo Info { get; }

            /// <inheritdoc />
            public object? Extract(IPropertyBase? property, PropertyInfo? propertyInfo, IEFDefinitionExtractContext context)
            {
                return _extract(property, propertyInfo, context);
            }
        }
    }
}