using System;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public sealed class AttributeDefinition<T, TAttr> : AttributeDefinitionBase<T, TAttr>
        where TAttr : DefinitionAttribute
    {
        /// <inheritdoc />
        public AttributeDefinition(string name, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
        }

        /// <inheritdoc />
        protected override T? GetValue(TAttr attr, DefinitionContext context)
        {
            return (T?)attr.Value;
        }
    }
}