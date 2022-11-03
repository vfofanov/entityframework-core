using System.Reflection;
using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Definitions
{
    public interface IMemberInfoDefinitionInfo
    {
        DefinitionInfo Info { get; }
        object? Extract(MemberInfo? member, IDefinitionExtractContext context);
    }
}