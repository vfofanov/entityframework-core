using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    // ReSharper disable once UnusedType.Global
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
            optionsBuilder.UseSqlServer();
            return new MainDbContext(optionsBuilder.Options);
        }
    }
}