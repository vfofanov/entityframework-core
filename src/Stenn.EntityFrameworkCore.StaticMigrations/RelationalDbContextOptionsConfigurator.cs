using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.StaticMigrations.Conventions;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public class RelationalDbContextOptionsConfigurator : IDbContextOptionsConfigurator
    {
        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder)
        {
            builder.ReplaceService<IMigrator, MigratorWithStaticMigrations>();
            builder.ReplaceService<IModelCustomizer, RelationalModelCustomizerWithConventions>();
        }
    }
}