using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Conventions;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Conventions;

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