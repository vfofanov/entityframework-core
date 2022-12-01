using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute.Migrations.Static;
using Stenn.EntityFrameworkCore.EntityConventions.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased.SqlServer;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute
{
    // ReSharper disable once UnusedType.Global
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
            
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
            
            return new MainDbContext(optionsBuilder.Options);
        }
    }
}