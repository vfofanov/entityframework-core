using System;
using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartGraphBuilderOptions
    {
        FlowchartGraphDirection Direction { get;  }
        List<FlowchartGraphGrouping> GraphGroupings { get; }
        Func<PropertyDefinitionRow, bool> PropertyFilter { get;  }
        Action<FlowchartStyleClass> InitAbstractEntityStyleClassAction { get; }
        
        IFlowchartEntityOptions Entity { get; }
        IFlowchartPropertyOptions Property { get; }
    }
}