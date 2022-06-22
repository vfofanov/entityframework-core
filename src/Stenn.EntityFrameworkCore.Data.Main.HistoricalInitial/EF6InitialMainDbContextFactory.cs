using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Main.Migrations.Static;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.EntityConventions.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased.SqlServer;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalInitial
{
    // ReSharper disable once UnusedType.Global
    public class EF6InitialMainDbContextFactory : IDesignTimeDbContextFactory<HistoricalInitialMainDbContext>
    {
        public HistoricalInitialMainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HistoricalInitialMainDbContext>();
            
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseEntityConventionsSqlServer(b =>
            {
                b.AddTriggerBasedCommonConventions();
            });
            
            optionsBuilder.UseStaticMigrationsSqlServer(b =>
                {
                    MainStaticMigrations.Init(b);
                    b.AddTriggerBasedEntityConventionsMigrationSqlServer();
                }
            );
            
            return new HistoricalInitialMainDbContext(optionsBuilder.Options);
        }
    }
}