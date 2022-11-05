using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFEntityDefinition<T> : Definition<T>, IEFEntityDefinition<T>
    {
        private readonly Func<IEntityType, DefinitionContext, T?> _extract;

        /// <inheritdoc />
        public EFEntityDefinition(DefinitionInfo<T> info, Func<IEntityType, DefinitionContext, T?> extract) 
            : base(info)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        public EFEntityDefinition(string name, Func<IEntityType, DefinitionContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        T? IEFEntityDefinition<T>.Extract(IEntityType type, DefinitionContext context)
        {
            return _extract(type, context);
        }
    }
}