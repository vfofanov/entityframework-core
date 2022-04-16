using System;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InitialMigrationAttribute : Attribute
    {
        /// <inheritdoc />
        public InitialMigrationAttribute(string[] removeMigrationRowIds)
        {
            RemoveMigrationRowIds = removeMigrationRowIds;
        }

        public string[] RemoveMigrationRowIds { get; }

    }
}