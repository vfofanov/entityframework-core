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
    public class MainWithoutAttributeDbContextFactory : IDesignTimeDbContextFactory<MainTypeRegistrationDbContext>
    {
        public MainTypeRegistrationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainTypeRegistrationDbContext>();
            
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseEntityConventionsSqlServer(b =>
            {
                b.AddTriggerBasedCommonConventions();
            });
            
            optionsBuilder.UseStaticMigrationsSqlServer(b =>
                {
                    MainWithoutAttributeStaticMigrations.Init(b);
                    b.AddTriggerBasedEntityConventionsMigrationSqlServer();
                }
            );
            
            return new MainTypeRegistrationDbContext(optionsBuilder.Options);
        }
    }
}