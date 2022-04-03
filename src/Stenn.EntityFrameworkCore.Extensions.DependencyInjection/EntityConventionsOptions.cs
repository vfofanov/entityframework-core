using System;
using Stenn.EntityConventions.Contacts;
using Stenn.EntityFrameworkCore.EntityConventions;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public sealed class EntityConventionsOptions
    {
        /// <summary>
        /// Initialize custom entity conventions
        /// </summary>
        public Action<IEntityConventionsBuilder>? InitEntityConventions { get; set; }

        /// <summary>
        /// Include common entity conventions
        /// </summary>
        public bool IncludeCommonConventions { get; set; } = true;
        
        public EntityConventionsCommonDefaultsOptions Defaults { get;  } = new();
    }
}