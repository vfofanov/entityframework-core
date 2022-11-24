using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public class FlowchartGraphGrouping
    {
        private readonly Action<object?, FlowchartStyleClass>? _fillStyle;

        public static FlowchartGraphGrouping Create<T>(DefinitionInfo<T> info, DefinitionColumnType columnType,
            Action<T?, FlowchartStyleClass>? fillStyle = null)
        {
            Action<object?, FlowchartStyleClass>? fillStyleAction = fillStyle == null ? null : (v, styleClass) => { fillStyle((T?)v, styleClass); };
            return new FlowchartGraphGrouping(info, columnType, fillStyleAction);
        }

        private FlowchartGraphGrouping(DefinitionInfo info, DefinitionColumnType columnType, Action<object?, FlowchartStyleClass>? fillStyle = null)
        {
            _fillStyle = fillStyle;
            Info = info;
            ColumnType = columnType;
        }

        public DefinitionInfo Info { get; }
        public DefinitionColumnType ColumnType { get; }

        public bool HasStyle => _fillStyle != null;

        public void FillStyle(object? value, FlowchartStyleClass styleClass)
        {
            _fillStyle?.Invoke(value, styleClass);
        }
    }
}