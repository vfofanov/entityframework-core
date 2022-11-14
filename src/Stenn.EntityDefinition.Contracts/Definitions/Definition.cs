using System;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public abstract class Definition<T> : IDefinition<T>
    {
        protected Definition(string name, Func<T, string>? convertToString = null)
            : this(new DefinitionInfo<T>(name, convertToString))
        {
        }

        protected Definition(DefinitionInfo<T> info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        /// <inheritdoc />
        public DefinitionInfo<T> Info { get; }
    }
}