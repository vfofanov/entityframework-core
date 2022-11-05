using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFEntityDefinition<T> : Definition<T>, IEFEntityDefinition<T>
    {
        private readonly Func<IEntityType,T?, DefinitionContext, T?> _extract;

        /// <inheritdoc />
        public EFEntityDefinition(DefinitionInfo<T> info, Func<IEntityType,T?, DefinitionContext, T?> extract) 
            : base(info)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        public EFEntityDefinition(string name, Func<IEntityType,T?, DefinitionContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        T? IEFEntityDefinition<T>.Extract(IEntityType type,T? parentValue, DefinitionContext context)
        {
            return _extract(type, parentValue, context);
        }
    }
}