using System;
using Stenn.EntityFrameworkCore.EntityConventions;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    public sealed class StaticMigrationsOptions
    {
        /// <summary>
        /// Initialize custom static migrations
        /// </summary>
        public Action<StaticMigrationBuilder>? InitMigrations { get; set; }
        
        /// <summary>
        /// Enable enums' tables migration
        /// </summary>
        public bool EnableEnumTables { get; set; } = true;

        /// <summary>
        /// Initialize custom entity conventions
        /// </summary>
        public Action<IEntityConventionsBuilder>? InitEntityConventions { get; set; }

        /// <summary>
        /// Include common entity conventions
        /// </summary>
        public bool IncludeCommonConventions { get; set; } = true;
    }
}