using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition
{
    public interface IDefinitionReader
    {
        DefinitionMap Read();
    }
}