using System;
using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartElementOptionsBase<TRow>:IFlowchartElementOptionsBase<TRow>
        where TRow : DefinitionRowBase
    {
        private Func<TRow, bool> _filter = _ => true;
        private readonly List<FlowchartGraphGroup> _graphGroups = new();

        public FlowchartGraphDirection GroupDirection { get; set; } = FlowchartGraphDirection.TB;
        
        public void SetFilter(Func<TRow, bool>? filter)
        {
            _filter = filter ?? (_ => true);
        }

        public void AddGraphGroup(FlowchartGraphGroup group)
        {
            _graphGroups.Add(group);
        }

        IReadOnlyCollection<FlowchartGraphGroup> IFlowchartElementOptionsBase<TRow>.GraphGroups =>_graphGroups;

        bool IFlowchartElementOptionsBase<TRow>.Filter(TRow row)
        {
            return _filter(row);
        }
        
    }
}