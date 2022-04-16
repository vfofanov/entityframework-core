using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Main.SplittedInitial
{
    // ReSharper disable once UnusedType.Global
    public class SplittedInitialMainDbContextFactory : IDesignTimeDbContextFactory<SplittedInitialMainDbContext>
    {
        public SplittedInitialMainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SplittedInitialMainDbContext>();
            
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
            
            return new SplittedInitialMainDbContext(optionsBuilder.Options);
        }
    }
}