using System;

namespace Stenn.DictionaryEntities
{
    /// <summary>
    /// Ignore property from Dictionary entity declaration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DictionaryEntityIgnoreAttribute : Attribute
    {
    }
}