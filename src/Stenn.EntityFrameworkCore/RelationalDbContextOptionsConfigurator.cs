using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class RelationalDbContextOptionsConfigurator : IDbContextOptionsConfigurator
    {
        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder)
        {
            builder.ReplaceService<IMigrator, MigratorWithStaticMigrations>();
        }
    }
}