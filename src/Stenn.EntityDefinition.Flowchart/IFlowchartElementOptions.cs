using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartElementOptions<in TRow> : IFlowchartElementOptionsBase<TRow>
        where TRow : DefinitionRowBase
    {
        DefinitionInfo<string> Id { get; }
        DefinitionInfo<string> Caption { get; }
    }
}