using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Flowchart;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Flowchart
{
    public sealed class EFFlowchartPropertyOptions : FlowchartPropertyOptionsBase, IFlowchartPropertyOptions
    {
        /// <inheritdoc />
        DefinitionInfo<object> IFlowchartPropertyOptions.Id => Id;

        /// <inheritdoc />
        DefinitionInfo<string> IFlowchartPropertyOptions.ItemId => ItemId;

        /// <inheritdoc />
        DefinitionInfo<string> IFlowchartPropertyOptions.Caption => Caption;

        /// <inheritdoc />
        DefinitionInfo<bool> IFlowchartPropertyOptions.IsNavigation => IsNavigation;

        /// <inheritdoc />
        DefinitionInfo<bool?> IFlowchartPropertyOptions.IsNavigationCollection => IsCollection;

        /// <inheritdoc />
        DefinitionInfo<bool?> IFlowchartPropertyOptions.IsOnDependent => IsOnDependent;

        /// <inheritdoc />
        DefinitionInfo<Type> IFlowchartPropertyOptions.TargetType => TargetType;

        /// <inheritdoc />
        DefinitionInfo<object> IFlowchartPropertyOptions.TargetId => TargetId;

        /// <inheritdoc />
        DefinitionInfo<string> IFlowchartPropertyOptions.RelationCaption => RelationCaption;

        public EFPropertyDefinition<object> Id { get; set; } = EFCommonDefinitions.Properties.Id;
        public EFPropertyDefinition<string> ItemId { get; set; } = EFCommonDefinitions.Properties.Name;
        public EFPropertyDefinition<string> Caption { get; set; } = EFCommonDefinitions.Properties.Name;
        public EFPropertyDefinition<bool> IsNavigation { get; set; } = EFCommonDefinitions.Properties.IsNavigation;
        public EFPropertyDefinition<bool?> IsCollection { get; set; } = EFCommonDefinitions.Properties.Navigation.IsNavigationCollection;
        public EFPropertyDefinition<bool?> IsOnDependent { get; set; } = EFCommonDefinitions.Properties.Navigation.IsOnDependent;
        public EFPropertyDefinition<Type> TargetType { get; set; } = EFCommonDefinitions.Properties.Navigation.TargetEntityType;
        public EFPropertyDefinition<object> TargetId { get; set; } = EFCommonDefinitions.Properties.Navigation.TargetPropertyId;
        public EFPropertyDefinition<string> RelationCaption { get; set; } = EFCommonDefinitions.Properties.Navigation.RelationCaption;
    }
}