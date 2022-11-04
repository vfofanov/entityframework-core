using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFEntityDefinitionInfo<T> : DefinitionInfo<T>, IEFEntityDefinitionInfo<T>
    {
        private readonly Func<IEntityType, IEFDefinitionExtractContext, T?> _extract;

        /// <inheritdoc />
        public EFEntityDefinitionInfo(string name, Func<IEntityType, IEFDefinitionExtractContext, T?> extract,
            Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        DefinitionInfo<T> IEFEntityDefinitionInfo<T>.Info => this;

        /// <inheritdoc />
        T? IEFEntityDefinitionInfo<T>.Extract(IEntityType type, IEFDefinitionExtractContext context)
        {
            return _extract(type, context);
        }
    }
}