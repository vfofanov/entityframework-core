using System;

namespace Stenn.DictionaryEntities
{
    /// <summary>
    ///     Ignore property from Dictionary entity declaration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class DictionaryEntityIgnoredPropertiesAttribute : Attribute
    {
        /// <inheritdoc />
        public DictionaryEntityIgnoredPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }

        public string[] PropertiesNames { get; }
    }
}