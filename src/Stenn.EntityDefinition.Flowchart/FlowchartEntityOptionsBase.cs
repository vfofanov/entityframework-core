using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartEntityOptionsBase : FlowchartElementOptionsBase<EntityDefinitionRow>
    {
        public bool AddInheritRelations { get; set; } = true;
    }
}