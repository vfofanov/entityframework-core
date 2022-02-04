using System;

namespace Stenn.DictionaryEntities
{
    /// <summary>
    /// Key property for Dictionary entity declaration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DictionaryEntityKeyAttribute : Attribute
    {
    }
}