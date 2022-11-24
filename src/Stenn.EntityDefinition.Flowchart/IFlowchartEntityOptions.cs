using System;
using Stenn.EntityDefinition.Contracts;
using Stenn.Shared.Mermaid.Flowchart;

namespace Stenn.EntityDefinition.Flowchart
{
    public interface IFlowchartEntityOptions
    {
        FlowchartGraphDirection GroupDirection { get;  }
        DefinitionInfo<string> Id { get;  }
        DefinitionInfo<string> Caption { get;  }
        DefinitionInfo<Type> Type { get;  }
        DefinitionInfo<Type> BaseType { get;  }
        DefinitionInfo<bool> IsAbstract { get;  }
    }
}