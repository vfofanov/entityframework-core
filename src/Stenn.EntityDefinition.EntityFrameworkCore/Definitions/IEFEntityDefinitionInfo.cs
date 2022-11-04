using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public interface IEFEntityDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(IEntityType type, IEFDefinitionExtractContext context);
    }
    
    public interface IEFEntityDefinitionInfo<T>:IEFEntityDefinitionInfo
    {
        /// <inheritdoc />
        DefinitionInfo IEFEntityDefinitionInfo.Info => Info;

        /// <inheritdoc />
        object? IEFEntityDefinitionInfo.Extract(IEntityType type, IEFDefinitionExtractContext context)
        {
            return Extract(type, context);
        }

        new DefinitionInfo<T> Info { get; }
        new T? Extract(IEntityType type, IEFDefinitionExtractContext context);
    }
}