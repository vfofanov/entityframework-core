#nullable enable
using System;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Flowchart;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Flowchart
{
    public static class EFFlowchartGraphGroupingExtensions
    {
        public static FlowchartGraphGroup ToFlowchartGraphGrouping<T>(this EFEntityDefinition<T> definition,
            Action<T?, FlowchartStyleClass>? fillStyle = null,
            Func<T?, string>? extractItemId = null,
            Func<T?, string?>? extractCaption = null, bool skipDuringClean = false)
        {
            return definition.Info.ToFlowchartGraphGroup(fillStyle,extractItemId, extractCaption, skipDuringClean);
        }

        public static FlowchartGraphGroup ToFlowchartGraphGrouping<T>(this EFPropertyDefinition<T> definition,
            Action<T?, FlowchartStyleClass>? fillStyle = null,
            Func<T?, string>? extractItemId = null,
            Func<T?, string?>? extractCaption = null, bool skipDuringClean = false)
        {
            return definition.Info.ToFlowchartGraphGroup(fillStyle,extractItemId, extractCaption, skipDuringClean);
        }
    }
}