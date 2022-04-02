using System;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public class SplittedMigrationsOptions : IEquatable<SplittedMigrationsOptions>
    {
        public SplittedMigrationsOptions(Type[] anchors)
        {
            Anchors = anchors ?? throw new ArgumentNullException(nameof(anchors));
            if (anchors.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(anchors));
            }
        }
        public Type[] Anchors { get; }

        /// <inheritdoc />
        public bool Equals(SplittedMigrationsOptions? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Anchors.Equals(other.Anchors);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((SplittedMigrationsOptions)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Anchors.GetHashCode();
        }
    }
}