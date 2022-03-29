using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Conventions;

namespace Stenn.EntityFrameworkCore.SqlServer.Conventions
{
    public sealed class ConventionsStaticMigrationFactorySqlServer : IConventionsStaticMigrationFactory
    {
        /// <inheritdoc />
        public IStaticSqlMigration Create(DbContext context)
        {
            return new ConventionsStaticMigrationSqlServer(context);
        }
    }
}