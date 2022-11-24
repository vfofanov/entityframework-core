﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Flowchart;
using Stenn.Shared.Mermaid.Flowchart;
using static Stenn.EntityDefinition.EntityFrameworkCore.EntityFrameworkDefinitionReaderOptions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Flowchart
{
    public sealed class EFFlowchartGraphBuilder : FlowchartGraphBuilder<EFFlowchartGraphBuilderOptions>
    {
        /// <inheritdoc />
        public EFFlowchartGraphBuilder(EFFlowchartGraphBuilderOptions options) 
            : base(options)
        {
        }

        public FlowchartGraph Build(DbContext dbContext)
        {
            return Build(dbContext.Model);
        }
        public FlowchartGraph Build(IModel model)
        {
            var map = model.GenerateMap(InitDefinitionModelReaderOptions);
            return Build(map);
        }

        private void InitDefinitionModelReaderOptions(IEntityFrameworkCoreDefinitionOptions options)
        {
            Options.InitReaderOptions(options);
            
            options.SetPropertiesFilter((e, p) => p.DeclaringType == e);
            options.ReaderOptions = ExcludeScalarProperties | ExcludeIgnoredProperties;

            options.TryAddEntityColumn(Options.Entity.Id);
            options.TryAddEntityColumn(Options.Entity.Caption);
            options.TryAddEntityColumn(Options.Entity.Type);
            options.TryAddEntityColumn(Options.Entity.BaseType);

            options.TryAddPropertyColumn(Options.Property.Id);
            options.TryAddPropertyColumn(Options.Property.ItemId);
            options.TryAddPropertyColumn(Options.Property.Caption);
            options.TryAddPropertyColumn(Options.Property.IsNavigation);
            options.TryAddPropertyColumn(Options.Property.IsCollection);
            options.TryAddPropertyColumn(Options.Property.IsOnDependent);

            options.TryAddPropertyColumn(Options.Property.TargetType);
            options.TryAddPropertyColumn(Options.Property.TargetId);
            options.TryAddPropertyColumn(Options.Property.RelationCaption);
            options.TryAddPropertyColumn(Options.Property.RelationTooltip);
        }
    }
}