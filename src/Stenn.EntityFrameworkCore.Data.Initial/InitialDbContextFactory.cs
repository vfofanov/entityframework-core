using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Data.Initial
{
    // ReSharper disable once UnusedType.Global
    public class InitialDbContextFactory : IDesignTimeDbContextFactory<InitialDbContext>
    {
        public InitialDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InitialDbContext>();
            
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseStaticMigrationsSqlServer(InitialStaticMigrations.Init);
            
            return new InitialDbContext(optionsBuilder.Options);
        }
    }
}