using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Data.Configurations;

namespace Stenn.EntityFrameworkCore.DbContext.Initial
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

            base.OnModelCreating(modelBuilder);
        }
    }
}