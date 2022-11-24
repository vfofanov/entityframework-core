using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public static class FlowchartGraphGroupingExtensions
    {
        public static FlowchartGraphGrouping ToFlowchartGraphGrouping<T>(this Definition<T> definition, DefinitionColumnType columnType,
            Action<T?, FlowchartStyleClass>? fillStyle = null)
        {
            return FlowchartGraphGrouping.Create(definition, columnType, fillStyle);
        }

        public static FlowchartGraphGrouping ToFlowchartGraphGrouping<T>(this DefinitionInfo<T> info, DefinitionColumnType columnType,
            Action<T?, FlowchartStyleClass>? fillStyle = null)
        {
            return FlowchartGraphGrouping.Create(info, columnType, fillStyle);
        }
    }
}