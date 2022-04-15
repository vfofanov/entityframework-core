using System;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    /// <summary>
    /// Mark Entity Framework migration to show that this migration has previous migrations splitted to another assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SplittedMigrationAttribute : Attribute
    {
        /// <inheritdoc />
        public SplittedMigrationAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
        }

        /// <summary>
        /// Special splitted migration that replace all previous migrations.
        /// If any of the previous migrations don't applied yet, they will be applied, but specified migration will be skipped, just add row to history
        /// </summary>
        public bool Initial { get; set; }

        /// <summary>
        /// DbContext type of splitted migrations marker
        /// </summary>
        public Type DbContextType { get; }
    }
}