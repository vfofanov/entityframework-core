using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Data.Main.Configurations;
using Stenn.EntityFrameworkCore.EntityConventions;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    public class MainDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        /// <inheritdoc />
        protected MainDbContext()
        {
        }

        /// <inheritdoc />
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CurrencyMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new ContactMap());
            modelBuilder.ApplyConfiguration(new Contact2Map());
            modelBuilder.ApplyConfiguration(new AnimalMap());
            modelBuilder.ApplyConfiguration(new CatMap());
            modelBuilder.ApplyConfiguration(new ElefantMap());
        }
    }
}