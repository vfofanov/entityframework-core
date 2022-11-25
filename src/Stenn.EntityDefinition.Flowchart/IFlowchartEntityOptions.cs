using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartEntityOptions : IFlowchartElementOptions<EntityDefinitionRow>
    {
        /// <summary>
        /// Add entities inheritance relations
        /// </summary>
        bool AddInheritRelations { get; }

        DefinitionInfo<Type> Type { get; }
        DefinitionInfo<Type> BaseType { get; }
        DefinitionInfo<bool> IsAbstract { get; }
    }
}