using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Conventions;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer
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