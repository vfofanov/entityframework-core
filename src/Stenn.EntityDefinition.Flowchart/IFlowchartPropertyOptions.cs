using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartPropertyOptions
    {
        /// <summary>
        /// Use individual graph node for draw property
        /// </summary>
        /// <returns></returns>
        bool DrawAsNode { get; }

        FlowchartGraphDirection GroupDirection { get; }
        DefinitionInfo<object> Id { get; }
        DefinitionInfo<string> ItemId { get; }
        DefinitionInfo<string> Caption { get; }
        DefinitionInfo<bool> IsNavigation { get; }
        DefinitionInfo<bool?> IsNavigationCollection { get; }
        DefinitionInfo<bool?> IsOnDependent { get; }
        DefinitionInfo<Type> TargetType { get; }
        DefinitionInfo<object> TargetId { get; }
        DefinitionInfo<string> RelationCaption { get; }
        DefinitionInfo<string> RelationTooltip { get; }
    }
}