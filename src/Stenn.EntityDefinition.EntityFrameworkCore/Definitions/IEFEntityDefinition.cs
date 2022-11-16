using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFEntityDefinition : IDefinition
    {
        object? Extract(IEntityType type, EntityDefinitionRow row, DefinitionContext context);
    }

    public interface IEFEntityDefinition<T> : IEFEntityDefinition, IDefinition<T>
    {
        /// <inheritdoc />
        object? IEFEntityDefinition.Extract(IEntityType type, EntityDefinitionRow row, DefinitionContext context)
        {
            return Extract(type, row, context);
        }

        new T? Extract(IEntityType type, EntityDefinitionRow row, DefinitionContext context);
    }
}