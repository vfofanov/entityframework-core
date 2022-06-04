using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.EntityConventions.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased.SqlServer;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.EF6Initial
{
    // ReSharper disable once UnusedType.Global
    public class HistoricalInitialMainDbContextFactory : IDesignTimeDbContextFactory<EF6InitialMainDbContext>
    {
        public EF6InitialMainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EF6InitialMainDbContext>();
            
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
            
            return new EF6InitialMainDbContext(optionsBuilder.Options);
        }
    }
}