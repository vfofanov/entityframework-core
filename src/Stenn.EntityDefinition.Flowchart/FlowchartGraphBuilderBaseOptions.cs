using System;
using System.Collections.Generic;
using System.Drawing;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartGraphBuilderBaseOptions<TEntityOptions, TPropertyOptions> : IFlowchartGraphBuilderOptions
        where TEntityOptions : class, IFlowchartEntityOptions, new()
        where TPropertyOptions : class, IFlowchartPropertyOptions, new()
    {
        private Func<PropertyDefinitionRow, bool> _propertyFilter = _ => true;
        private Action<FlowchartStyleClass> _initAbstractEntityStyleClassAction = InitArstractEntityStyleClassDefault;
        private Action<FlowchartStyleClass> _initRelationNodeStyleClassActionAction = InitRelationNodeStyleClassDefault;
        
        public bool DrawRelationAsNode { get; set; }
        
        public FlowchartGraphDirection Direction { get; set; } = FlowchartGraphDirection.LR;

        public List<FlowchartGraphGrouping> GraphGroupings { get; } = new();

        Func<PropertyDefinitionRow, bool> IFlowchartGraphBuilderOptions.PropertyFilter => _propertyFilter;

        Action<FlowchartStyleClass> IFlowchartGraphBuilderOptions.InitAbstractEntityStyleClassAction => _initAbstractEntityStyleClassAction;

        public void SetPropertyFilter(Func<PropertyDefinitionRow, bool> filter)
        {
            _propertyFilter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public void InitArstractEntityStyleClass(Action<FlowchartStyleClass> init)
        {
            _initAbstractEntityStyleClassAction = init ?? throw new ArgumentNullException(nameof(init));
        }

        private static void InitArstractEntityStyleClassDefault(FlowchartStyleClass styleClass)
        {
            //styleClass.SetModifier("font-weight", "bold");
            styleClass.SetModifier("font-style", "italic");
            styleClass.SetModifier("color", ColorTranslator.ToHtml(Color.Blue));
        }
        
        public void InitRelationNodeStyleClass(Action<FlowchartStyleClass> init)
        {
            _initRelationNodeStyleClassActionAction = init ?? throw new ArgumentNullException(nameof(init));
        }

        public Action<FlowchartStyleClass> InitRelationNodeStyleClassAction => _initRelationNodeStyleClassActionAction;

        private static void InitRelationNodeStyleClassDefault(FlowchartStyleClass styleClass)
        {
            //styleClass.SetModifier("font-style", "italic");
            styleClass.SetModifier("fill", ColorTranslator.ToHtml(Color.Azure));
            styleClass.SetModifier("font-size", "80%");
            styleClass.SetModifier("color", ColorTranslator.ToHtml(Color.DimGray));
        }
        
        IFlowchartEntityOptions IFlowchartGraphBuilderOptions.Entity => Entity;
        public TEntityOptions Entity { get; } = new();

        IFlowchartPropertyOptions IFlowchartGraphBuilderOptions.Property => Property;
        public TPropertyOptions Property { get; } = new();
    }
}