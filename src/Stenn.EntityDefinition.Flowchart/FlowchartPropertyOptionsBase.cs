using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public abstract class FlowchartPropertyOptionsBase
    {
        public bool DrawAsNode { get; set; } = true;

        public FlowchartGraphDirection GroupDirection { get; set; } = FlowchartGraphDirection.TB;
    }
}