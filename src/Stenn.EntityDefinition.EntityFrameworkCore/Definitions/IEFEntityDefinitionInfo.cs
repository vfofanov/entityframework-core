using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public interface IEFEntityDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IEntityType type, IEFDefinitionExtractContext context);
    }
}