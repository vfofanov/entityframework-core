using System;
using System.ComponentModel;

namespace Stenn.EntityConventions.Contacts
{
    public sealed class SourceSystemIdOptions : Attribute
    {
        /// <summary>
        /// Max length. Default:50 
        /// </summary>
        [DefaultValue(50)]
        public int MaxLength { get; init; } = 50;

        /// <summary>
        /// Is unicode. Default:false 
        /// </summary>
        [DefaultValue(false)]
        public bool IsUnicode { get; init; }

        /// <summary>
        /// Has index. Default:true 
        /// </summary>
        [DefaultValue(true)]
        public bool HasIndex { get; init; } = true;

        /// <summary>
        /// Has value generator
        /// </summary>
        public bool HasValueGenerator { get; init; } = true;

        /// <summary>
        /// Value generator type if null standard value generator will be used
        /// </summary>
        public Type? Generator { get; init; } = null;
    }
}