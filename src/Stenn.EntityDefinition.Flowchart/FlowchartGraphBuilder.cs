using System;
using System.Collections.Generic;
using System.Linq;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.Shared.Mermaid.Flowchart;
using Stenn.Shared.Reflection;
using static Stenn.Shared.Mermaid.Flowchart.FlowchartRelationLineEnding;

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

            var abstractStylelClassId = "G_AbstractEntityStyleClass";
            {
                var abstractStylelClass = graph.GetOrAddStyleClass(abstractStylelClassId);
                Options.InitAbstractEntityStyleClassAction(abstractStylelClass);
            }
            var relationNodeStyleClassId = "G_RelationNodeStyleClass";

            if (Options.DrawRelationAsNode)
            {
                var relationNodeStyleClass = graph.GetOrAddStyleClass(relationNodeStyleClassId);
                Options.InitRelationNodeStyleClassAction(relationNodeStyleClass);
            }

            var entityItemIds = new Dictionary<Type, string>(map.Entities.Count);

            string GetEntityItemId(EntityDefinitionRow entityDefinitionRow)
            {
                var entityType = entityDefinitionRow.Get(Options.Entity.Type);
                if (entityType == null)
                {
                    throw new ApplicationException("Clr type is null for entity");
                }
                return entityItemIds[entityType];
            }

            int relationNodesCount = 0;

            void AddRelation(string leftItemId, string rightItemId, string? caption = null, string? tooltip = null,
                FlowchartRelationLineEnding leftItemEnding = None,
                FlowchartRelationLineStyle lineStyle = FlowchartRelationLineStyle.Line, int lineLength = 0,
                FlowchartRelationLineEnding rightItemEnding = Arrow)
            {
                if (Options.DrawRelationAsNode)
                {
                    var relationItemId = "Relation_" + ++relationNodesCount;

                    var leftParentId = leftItemId;
                    var rightParentId = rightItemId;
                    do
                    {
                        var leftItem = leftParentId == null ? null : graph.FindItem(leftParentId);
                        var rightItem = rightParentId == null ? null : graph.FindItem(rightParentId);
                        leftParentId = leftItem?.Parent?.Id;
                        rightParentId = rightItem?.Parent?.Id;
                    } while (leftParentId != rightParentId);


                    graph.GetOrAdd(relationItemId, caption, parentItemId: leftParentId,
                        styleClassId: relationNodeStyleClassId, shape: FlowchartShape.Hexagon);

                    if (tooltip is not null)
                    {
                        graph.AddItemInteractionTooltip(relationItemId, tooltip);
                    }
                    
                    graph.AddRelation(leftItemId, relationItemId, null, leftItemEnding,
                        lineStyle, lineLength / 2, None);

                    graph.AddRelation(relationItemId, rightItemId, null, None,
                        lineStyle, lineLength / 2, rightItemEnding);
                }
                else
                {
                    graph.AddRelation(leftItemId, rightItemId, caption, leftItemEnding,
                        lineStyle, lineLength, rightItemEnding);
                }
            }

            #region Entities
            var entityGroupings = Options.GraphGroupings.Where(g => g.ColumnType == DefinitionColumnType.Entity).ToList();
            foreach (var entityRow in map.Entities)
            {
                var parentItemId = AddOrGetGroupingItemId(graph, entityGroupings, entityRow, null, "E", Options.Entity.GroupDirection);

                var itemId = entityRow.Get(Options.Entity.Id);
                if (itemId == null)
                {
                    throw new ApplicationException("Id is null for entity");
                }
                string? itemStyleClassId = null;
                if (entityRow.GetValueOrDefault(Options.Entity.IsAbstract))
                {
                    itemStyleClassId = abstractStylelClassId;
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
            foreach (var entityRow in map.Entities)
            {
                var entityType = entityRow.Get(Options.Entity.Type);
                var entityBaseType = entityRow.GetValueOrDefault(Options.Entity.BaseType);
                if (entityBaseType == null)
                {
                    continue;
                }
                var leftItemId = entityItemIds[entityBaseType];
                var rightItemId = GetEntityItemId(entityRow);
                var toolTip = $"{entityType?.HumanizeName()} inherits from {entityBaseType.HumanizeName()}";

                AddRelation(leftItemId, rightItemId, "<<inherit>>", tooltip: toolTip,
                    lineStyle: FlowchartRelationLineStyle.BoldLine);
            }
            #endregion

            Dictionary<object, string>? propertyItemIds = null;
            if (Options.Property.DrawAsNode)
            {
                propertyItemIds = new Dictionary<object, string>(map.Entities.Select(e => e.Properties.Count).Sum());
                var propertyGroupings = Options.GraphGroupings.Where(g => g.ColumnType == DefinitionColumnType.Property).ToList();

                #region Property Nodes
                foreach (var entityRow in map.Entities)
                {
                    var entityItemId = GetEntityItemId(entityRow);
                    foreach (var propertyRow in entityRow.Properties.Where(Options.PropertyFilter))
                    {
                        var parentItemId = AddOrGetGroupingItemId(graph, propertyGroupings, propertyRow, entityItemId, "P", Options.Property.GroupDirection);

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
            }

            #region Property Relations
            string GetPropertyLeftItemId(string entityItemId, PropertyDefinitionRow propertyRow)
            {
                if (propertyItemIds == null)
                {
                    return entityItemId;
                }
                var propertyId = propertyRow.Get(Options.Property.Id);
                if (propertyId == null)
                {
                    throw new ApplicationException("Id is null for property");
                }
                return propertyItemIds[propertyId];
            }

            string GetPropertyRightItemId(object? propertyTargetId,
                PropertyDefinitionRow propertyRow)
            {
                if (propertyTargetId is not null && propertyItemIds is not null)
                {
                    return propertyItemIds[propertyTargetId];
                }

                var targetEntityType = propertyRow.Get(Options.Property.TargetType);
                if (targetEntityType is null)
                {
                    throw new ApplicationException("Target entity type is null for property");
                }
                return entityItemIds[targetEntityType];
            }


            foreach (var entityRow in map.Entities)
            {
                var entityItemId = GetEntityItemId(entityRow);

                foreach (var propertyRow in entityRow.Properties
                             .Where(p => p.GetValueOrDefault(Options.Property.IsNavigation) && Options.PropertyFilter(p)))
                {
                    var propertyTargetId = propertyRow.Get(Options.Property.TargetId);
                    var propertyIsOnDependent = propertyRow.Get(Options.Property.IsOnDependent);
                    if (propertyTargetId != null && propertyIsOnDependent == false)
                    {
                        //NOTE: Nav properties registered twice in EF, so we just skip one 
                        continue;
                    }

                    var leftItemId = GetPropertyLeftItemId(entityItemId, propertyRow);
                    var rightItemId = GetPropertyRightItemId(propertyTargetId, propertyRow);

                    var caption = propertyRow.Get(Options.Property.RelationCaption);
                    var toolTip = propertyRow.GetValueOrDefault(Options.Property.RelationTooltip);
                    AddRelation(leftItemId, rightItemId, caption, toolTip, 
                        leftItemEnding: propertyTargetId != null ? Arrow : None);
                }
            }
            #endregion

            return graph;
        }

        private static string? AddOrGetGroupingItemId(
            FlowchartGraph graph,
            IReadOnlyCollection<FlowchartGraphGrouping> groupings,
            DefinitionRowBase row, string? parentItemId, string prefix,
            FlowchartGraphDirection direction)
        {
            if (groupings.Count <= 0)
            {
                return parentItemId;
            }

            foreach (var grouping in groupings)
            {
                var groupValue = row.Get(grouping.Info);
                var groupCaption = grouping.Info.ConvertToString(groupValue) ?? "NULL";

                var groupItemId = parentItemId ?? prefix + "_" + groupCaption;

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
    }
}