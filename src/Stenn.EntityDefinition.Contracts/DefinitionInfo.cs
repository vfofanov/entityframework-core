using System;

namespace Stenn.EntityDefinition.Contracts
{
    public abstract class DefinitionInfo
    {
        internal DefinitionInfo(string name)
        {
            Name = name;
        }

        public abstract Type ValueType { get; }

        public virtual string? ConvertToString(object? val)
        {
            return val?.ToString();
        }

        public string Name { get; }
    }
}