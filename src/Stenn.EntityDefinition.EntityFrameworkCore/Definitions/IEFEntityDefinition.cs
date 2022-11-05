using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFEntityDefinition : IDefinition
    {
        object? Extract(IEntityType type, object? parentValue, DefinitionContext context);
    }

    public interface IEFEntityDefinition<T> : IEFEntityDefinition, IDefinition<T>
    {
        /// <inheritdoc />
        object? IEFEntityDefinition.Extract(IEntityType type, object? parentValue, DefinitionContext context)
        {
            return parentValue is null 
                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                ? Extract(type, default(T?), context) 
                : Extract(type,(T?) parentValue, context);
        }

        T? Extract(IEntityType type, T? parentValue, DefinitionContext context);
    }
}