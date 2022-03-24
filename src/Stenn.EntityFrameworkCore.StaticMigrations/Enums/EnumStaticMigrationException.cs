using System;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Enums
{
    public class EnumStaticMigrationException : Exception
    {
        /// <inheritdoc />
        public EnumStaticMigrationException(string? message = null)
            : base(message)
        {
        }

        /// <inheritdoc />
        public EnumStaticMigrationException(string? message, Exception? innerException) 
            : base(message, innerException)
        {
        }
    }
}