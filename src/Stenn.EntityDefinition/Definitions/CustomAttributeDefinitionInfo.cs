using System;

namespace Stenn.EntityDefinition.Definitions
{
    public sealed class CustomAttributeDefinitionInfo<T, TAttr> : AttributeDefinitionInfoBase<T, TAttr> where TAttr : Attribute
    {
        private readonly Func<TAttr, T?> _extract;

        /// <inheritdoc />
        public CustomAttributeDefinitionInfo(string name, Func<TAttr, T?> extract, Func<T, string>? convertToString = null)
            : base(name, convertToString)
        {
            _extract = extract ?? throw new ArgumentNullException(nameof(extract));
        }

        /// <inheritdoc />
        protected override T? GetValue(TAttr attr)
        {
            return _extract(attr);
        }
    }
}