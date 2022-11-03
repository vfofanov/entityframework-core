using System;
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
            return new EFPropertyDefinition(info, (property, context) => info.Extract(property.PropertyInfo, context));
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
            private readonly Func<IProperty, IEFDefinitionExtractContext, object?> _extract;

            internal EFPropertyDefinition(DefinitionInfo info, Func<IProperty, IEFDefinitionExtractContext, object?> extract)
            {
                _extract = extract;
                Info = info;
            }

            /// <inheritdoc />
            public DefinitionInfo Info { get; }

            /// <inheritdoc />
            public object? Extract(IProperty type, IEFDefinitionExtractContext context)
            {
                return _extract(type, context);
            }
        }
    }
}