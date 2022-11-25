using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public static class FlowchartGraphGroupExtensions
    {
        public static FlowchartGraphGroup ToFlowchartGraphGroup<T>(this Definition<T> definition,
            Action<T?, FlowchartStyleClass>? fillStyle = null,
            Func<T?, string>? extractItemId = null,
            Func<T?, string?>? extractCaption = null, bool skipDuringClean = false)
        {
            return FlowchartGraphGroup.Create(definition, fillStyle, extractItemId, extractCaption, skipDuringClean);
        }

        public static FlowchartGraphGroup ToFlowchartGraphGroup<T>(this DefinitionInfo<T> info,
            Action<T?, FlowchartStyleClass>? fillStyle = null,
            Func<T?, string>? extractItemId = null,
            Func<T?, string?>? extractCaption = null, bool skipDuringClean = false)
        {
            return FlowchartGraphGroup.Create(info, fillStyle, extractItemId, extractCaption, skipDuringClean);
        }
    }
}