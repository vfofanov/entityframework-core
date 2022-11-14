namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public interface IDefinition
    {
        DefinitionInfo Info { get; }
    }

    public interface IDefinition<T> : IDefinition
    {
        /// <inheritdoc />
        DefinitionInfo IDefinition.Info => Info;

        new DefinitionInfo<T> Info { get; }
    }
}