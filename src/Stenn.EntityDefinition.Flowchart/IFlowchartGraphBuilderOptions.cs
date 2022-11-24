using System;
using System.Collections.Generic;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartGraphBuilderOptions
    {
        /// <summary>
        /// Use individual graph node for draw property's relation
        /// </summary>
        public bool DrawRelationAsNode { get; }

        FlowchartGraphDirection Direction { get;  }
        List<FlowchartGraphGrouping> GraphGroupings { get; }
        Func<PropertyDefinitionRow, bool> PropertyFilter { get;  }

        Action<FlowchartStyleClass> InitAbstractEntityStyleClassAction { get; }
        Action<FlowchartStyleClass> InitRelationNodeStyleClassAction { get; }
        
        IFlowchartEntityOptions Entity { get; }
        IFlowchartPropertyOptions Property { get; }
    }
}