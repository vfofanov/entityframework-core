using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public abstract class EntityDefinitionInfo<T> : DefinitionInfo<T>, IEFEntityDefinitionInfo
    {
        /// <inheritdoc />
        public EntityDefinitionInfo(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
        }

        /// <inheritdoc />
        DefinitionInfo IEFEntityDefinitionInfo.Info => this;

        /// <inheritdoc />
        object? IEFEntityDefinitionInfo.Extract(IEntityType type, IEFDefinitionExtractContext context)
        {
            return Extract(type, context);
        }

        protected abstract T? Extract(IEntityType type, IEFDefinitionExtractContext context);
    }
}