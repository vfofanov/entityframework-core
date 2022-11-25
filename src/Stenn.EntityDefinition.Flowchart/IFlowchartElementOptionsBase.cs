using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartElementOptionsBase<in TRow>
        where TRow : DefinitionRowBase
    {
        IReadOnlyCollection<FlowchartGraphGroup> GraphGroups { get; }
        FlowchartGraphDirection GroupDirection { get; }

        public bool Filter(TRow row);
    }
}