using System;
using System.ComponentModel;

namespace Stenn.EntityConventions.Contacts
{
    public sealed class SourceSystemIdOptionsAttribute : Attribute
    {
        public static readonly SourceSystemIdOptionsAttribute Default = new();

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
    }
}