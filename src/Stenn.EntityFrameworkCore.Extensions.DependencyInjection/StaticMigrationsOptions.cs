using System;

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
        /// Entity conventions options
        /// </summary>
        public EntityConventionsOptions ConventionsOptions { get; } = new();
    }
}