#nullable enable
using System;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Flowchart;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Flowchart
{
    public static class EFFlowchartGraphGroupingExtensions
    {
        public static FlowchartGraphGrouping ToFlowchartGraphGrouping<T>(this EFEntityDefinition<T> definition,
            Action<T?, FlowchartStyleClass>? fillStyle = null)
        {
            return definition.Info.ToFlowchartGraphGrouping(DefinitionColumnType.Entity, fillStyle);
        }

        public static FlowchartGraphGrouping ToFlowchartGraphGrouping<T>(this EFPropertyDefinition<T> definition,
            Action<T?, FlowchartStyleClass>? fillStyle = null)
        {
            return definition.Info.ToFlowchartGraphGrouping(DefinitionColumnType.Property, fillStyle);
        }
    }
}