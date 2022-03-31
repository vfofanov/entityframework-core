using System;
using System.ComponentModel;

namespace Stenn.EntityConventions.Contacts
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SourceSystemIdOptions : StringPropertyOptions
    {
        /// <inheritdoc />
        public SourceSystemIdOptions()
        {
            IndexIsUnique = true;
        }

        /// <summary>
        /// Has value generator
        /// </summary>
        public bool HasValueGenerator { get; set; } = true;

        /// <summary>
        /// Value generator type if null standard value generator will be used
        /// </summary>
        public Type? Generator { get; set; } = null;
    }
}