using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFEntityDefinition : IDefinition
    {
        object? Extract(IEntityType type, DefinitionContext context);
    }

    public interface IEFEntityDefinition<T> : IEFEntityDefinition, IDefinition<T>
    {
        /// <inheritdoc />
        object? IEFEntityDefinition.Extract(IEntityType type, DefinitionContext context)
        {
            return Extract(type, context);
        }

        new T? Extract(IEntityType type, DefinitionContext context);
    }
}