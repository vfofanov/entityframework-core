using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFEntityDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IEntityType type, IEFDefinitionExtractContext context);
    }
}