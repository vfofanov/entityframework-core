using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stenn.EntityFrameworkCore.DbContext.Initial
{
    // ReSharper disable once UnusedType.Global
    public class InitialDbContextFactory : IDesignTimeDbContextFactory<InitialDbContext>
    {
        public InitialDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InitialDbContext>();
            optionsBuilder.UseSqlServer();
            return new InitialDbContext(optionsBuilder.Options);
        }
    }
}