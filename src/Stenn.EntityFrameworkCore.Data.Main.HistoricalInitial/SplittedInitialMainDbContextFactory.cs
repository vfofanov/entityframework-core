using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalInitial
{
    // ReSharper disable once UnusedType.Global
    public class HistoricalInitialMainDbContextFactory : IDesignTimeDbContextFactory<HistoricalInitialMainDbContext>
    {
        public HistoricalInitialMainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HistoricalInitialMainDbContext>();
            
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseStaticMigrationsSqlServer(options =>
                {
                    options.InitMigrations = MainStaticMigrations.Init;
                    options.ConventionsOptions.InitEntityConventions = b =>
                    {
                        b.AddTriggerBasedCommonConventions();
                    };
                }
            );
            
            return new HistoricalInitialMainDbContext(optionsBuilder.Options);
        }
    }
}