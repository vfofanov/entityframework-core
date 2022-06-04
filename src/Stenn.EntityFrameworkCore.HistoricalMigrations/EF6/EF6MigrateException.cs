using System;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations.EF6
{
    public sealed class EF6MigrateException : Exception
    {
        /// <inheritdoc />
        public EF6MigrateException()
        {
        }

        /// <inheritdoc />
        public EF6MigrateException(string? message) 
            : base(message)
        {
        }

        /// <inheritdoc />
        public EF6MigrateException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}