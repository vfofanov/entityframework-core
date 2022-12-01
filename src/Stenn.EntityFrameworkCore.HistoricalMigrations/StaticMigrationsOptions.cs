using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations
{
    public sealed class HistoricalMigrationsOptions : IEquatable<HistoricalMigrationsOptions>
    {
        /// <summary>
        /// Use full migrations history and ignore initial migrations
        /// </summary>
        public bool MigrateFromFullHistory { get; init; }

        public Type? DbContextType { get; set; }

        /// <inheritdoc />
        public bool Equals(HistoricalMigrationsOptions? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return MigrateFromFullHistory == other.MigrateFromFullHistory;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is HistoricalMigrationsOptions other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MigrateFromFullHistory.GetHashCode();
        }
    }
}