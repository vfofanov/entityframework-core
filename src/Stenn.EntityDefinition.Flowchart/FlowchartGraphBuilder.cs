using System;
using System.Collections.Generic;
using System.Linq;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.Shared.Mermaid;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartGraphBuilder<TOptions>
        where TOptions : IFlowchartGraphBuilderOptions
    {
        public FlowchartGraphBuilder(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }

        protected FlowchartGraph Build(IDefinitionMap map)
        {
            var graph = new FlowchartGraph { Direction = Options.Direction };
            var abstractStylelClass = graph.GetOrAddStyleClass("G_AbstractEntityStyleClass");
            Options.InitAbstractEntityStyleClassAction(abstractStylelClass);

            var entityItemIds = new Dictionary<Type, string>(map.Entities.Count);

            string? AddOrGetGroupingItemId(IReadOnlyCollection<FlowchartGraphGrouping> groupings,
                DefinitionRowBase row, string? parentItemId, string prefix, FlowchartGraphDirection direction)
            {
                if (groupings.Count <= 0)
                {
                    return parentItemId;
                }

                foreach (var grouping in groupings)
                {
                    var groupValue = row.Get(grouping.Info);
                    var groupCaption = grouping.Info.ConvertToString(groupValue) ?? "NULL";

                    var groupItemId = parentItemId ?? prefix + "_" + MermaidHelper.EscapeString(groupCaption);

                    var groupGraphItem = graph.FindItem(groupItemId);
                    if (groupGraphItem is not null)
                    {
                        parentItemId = groupItemId;
                    }
                    else
                    {
                        string? styleClassId = null;
                        if (grouping.HasStyle)
                        {
                            styleClassId = groupItemId + "StyleClass";
                            var styleClass = graph.GetOrAddStyleClass(styleClassId);
                            grouping.FillStyle(groupValue, styleClass);
                        }
                        graph.GetOrAdd(groupItemId, groupCaption, parentItemId: parentItemId, styleClassId: styleClassId, direction: direction);
                        parentItemId = groupItemId;
                    }
                }
                return parentItemId;
            }

            void AddRelation(string leftItemId, string rightItemId, string? caption = null,
                FlowchartRelationLineEnding leftItemEnding = FlowchartRelationLineEnding.None,
                FlowchartRelationLineStyle lineStyle = FlowchartRelationLineStyle.Line,
                int lineLength = 0,
                FlowchartRelationLineEnding rightItemEnding = FlowchartRelationLineEnding.Arrow)
            {
                graph.AddRelation(leftItemId, rightItemId, caption, leftItemEnding,
                    lineStyle, lineLength, rightItemEnding);
            }

            #region Entities
            var entityGroupings = Options.GraphGroupings.Where(g => g.ColumnType == DefinitionColumnType.Entity).ToList();
            foreach (var entityRow in map.Entities)
            {
                var parentItemId = AddOrGetGroupingItemId(entityGroupings, entityRow, null, "E", Options.Entity.GroupDirection);

                var itemId = entityRow.Get(Options.Entity.Id);
                if (itemId == null)
                {
                    throw new ApplicationException("Id is null for entity");
                }
                string? itemStyleClassId = null;
                if (entityRow.GetValueOrDefault(Options.Entity.IsAbstract))
                {
                    itemStyleClassId = abstractStylelClass;
                }

                var itemCaption = entityRow.Get(Options.Entity.Caption);
                graph.GetOrAdd(itemId, itemCaption, parentItemId: parentItemId,
                    styleClassId: itemStyleClassId, direction: Options.Property.GroupDirection);

                var entityType = entityRow.Get(Options.Entity.Type);
                if (entityType == null)
                {
                    throw new ApplicationException("Clr type is null for entity");
                }
                entityItemIds.Add(entityType, itemId);
            }
            #endregion

            #region Relations: Inheritance
            foreach (var entityRow in map.Entities.Where(e => e.GetValueOrDefault(Options.Entity.BaseType) != null))
            {
                var entityType = entityRow.Get(Options.Entity.Type);
                if (entityType == null)
                {
                    throw new ApplicationException("Clr type is null for entity");
                }
                var entityBaseType = entityRow.Get(Options.Entity.BaseType);
                if (entityBaseType == null)
                {
                    throw new ApplicationException("Clr base type is null for entity");
                }

                var leftItemId = entityItemIds[entityBaseType];
                var rightItemId = entityItemIds[entityType];

                AddRelation(leftItemId, rightItemId, "&lt&ltinherit&gt&gt",
                    lineStyle: FlowchartRelationLineStyle.BoldLine);
            }
            #endregion

            var propertyGroupings = Options.GraphGroupings.Where(g => g.ColumnType == DefinitionColumnType.Property).ToList();
            var propertyItemIds = new Dictionary<object, string>(map.Entities.Select(e => e.Properties.Count).Sum());

            #region Properties
            foreach (var entityRow in map.Entities)
            {
                foreach (var propertyRow in entityRow.Properties.Where(Options.PropertyFilter))
                {
                    var declaringType = entityRow.Get(Options.Entity.Type);
                    if (declaringType == null)
                    {
                        throw new ApplicationException("Clr type is null for entity");
                    }

                    var entityItemId = entityItemIds[declaringType];
                    var parentItemId = AddOrGetGroupingItemId(propertyGroupings, propertyRow, entityItemId, "P", Options.Property.GroupDirection);

                    var itemId = propertyRow.Get(Options.Property.ItemId);
                    if (itemId == null)
                    {
                        throw new ApplicationException("Id is null for property");
                    }
                    itemId = entityItemId + "_" + itemId;

                    string? itemStyleClassId = null;
                    var propertyShape = FlowchartShape.Box;
                    if (!propertyRow.GetValueOrDefault(Options.Property.IsNavigation))
                    {
                        propertyShape = FlowchartShape.BoxRoundEdges;
                    }
                    if (propertyRow.GetValueOrDefault(Options.Property.IsNavigationCollection) == true)
                    {
                        propertyShape = FlowchartShape.Subroutine;
                    }

                    var itemCaption = propertyRow.Get(Options.Property.Caption);

                    graph.GetOrAdd(itemId, itemCaption, parentItemId: parentItemId,
                        styleClassId: itemStyleClassId, shape: propertyShape);

                    var propertyId = propertyRow.Get(Options.Property.Id);
                    if (propertyId == null)
                    {
                        throw new ApplicationException("Id is null for property");
                    }
                    propertyItemIds.Add(propertyId, itemId);
                }
            }
            #endregion

            #region Relations: Navigation properties
            foreach (var entityRow in map.Entities)
            {
                foreach (var propertyRow in entityRow.Properties
                             .Where(p => p.GetValueOrDefault(Options.Property.IsNavigation) && Options.PropertyFilter(p)))
                {
                    var propertyId = propertyRow.Get(Options.Property.Id);
                    if (propertyId == null)
                    {
                        throw new ApplicationException("Id is null for property");
                    }

                    var propertyTargetId = propertyRow.Get(Options.Property.TargetId);

                    var propertyIsOnDependent = propertyRow.Get(Options.Property.IsOnDependent);
                    if (propertyTargetId != null && propertyIsOnDependent == false)
                    {
                        //NOTE: Nav properties registered twice in EF, so we just skip one 
                        continue;
                    }

                    var leftItemId = propertyItemIds[propertyId];
                    string rightItemId;
                    if (propertyTargetId is not null)
                    {
                        rightItemId = propertyItemIds[propertyTargetId];
                    }
                    else
                    {
                        var targetEntityType = propertyRow.Get(Options.Property.TargetType);
                        if (targetEntityType is null)
                        {
                            throw new ApplicationException("Target entity type is null for property");
                        }
                        rightItemId = entityItemIds[targetEntityType];
                    }
                    var caption = propertyRow.Get(Options.Property.RelationCaption);
                    AddRelation(leftItemId, rightItemId, caption);
                }
            }
            #endregion

            return graph;
        }
    }
}