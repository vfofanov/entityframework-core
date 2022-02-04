using System;

namespace Stenn.DictionaryEntities
{
    /// <summary>
    /// Ignore property from Dictionary entity declaration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class DictionaryEntityKeyPropertiesAttribute : Attribute
    {
        /// <inheritdoc />
        public DictionaryEntityKeyPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }
            
        public string[] PropertiesNames { get; }

    }
}