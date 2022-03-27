using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;

namespace Stenn.EntityFrameworkCore.SqlServer
{
    public sealed class EnumsStaticMigrationFactorySqlServer : IEnumsStaticMigrationFactory
    {
        /// <inheritdoc />
        public IStaticSqlMigration Create(DbContext context, string schemaName = "enum")
        {
            return new EnumsStaticMigrationSqlServer(context, schemaName);
        }
    }
}