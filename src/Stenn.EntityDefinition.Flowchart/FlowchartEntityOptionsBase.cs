using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartEntityOptionsBase
    {
        public FlowchartGraphDirection GroupDirection { get; set; } = FlowchartGraphDirection.TB;
    }
}