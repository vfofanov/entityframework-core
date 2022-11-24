using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Flowchart;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Flowchart
{
    public sealed class EFFlowchartEntityOptions : FlowchartEntityOptionsBase, IFlowchartEntityOptions
    {
        /// <inheritdoc />
        DefinitionInfo<string> IFlowchartEntityOptions.Id => Id;

        /// <inheritdoc />
        DefinitionInfo<string> IFlowchartEntityOptions.Caption => Caption;

        /// <inheritdoc />
        DefinitionInfo<Type> IFlowchartEntityOptions.Type => Type;

        /// <inheritdoc />
        DefinitionInfo<Type> IFlowchartEntityOptions.BaseType => BaseType;

        /// <inheritdoc />
        DefinitionInfo<bool> IFlowchartEntityOptions.IsAbstract => IsAbstract;

        public EFEntityDefinition<string> Id { get; set; } = EFCommonDefinitions.Entities.Name;
        public EFEntityDefinition<string> Caption { get; set; } = EFCommonDefinitions.Entities.Name;
        public EFEntityDefinition<Type> Type { get; set; } = EFCommonDefinitions.Entities.EntityType;
        public EFEntityDefinition<Type> BaseType { get; set; } = EFCommonDefinitions.Entities.BaseEntityType;
        public EFEntityDefinition<bool> IsAbstract { get; set; } = EFCommonDefinitions.Entities.IsAbstract;
    }
}