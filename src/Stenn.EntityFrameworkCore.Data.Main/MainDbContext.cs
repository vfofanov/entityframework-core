using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Conventions;
using Stenn.EntityFrameworkCore.Data.Main.Configurations;

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

            modelBuilder.ApplyConventions(builder =>
            {
                builder.AddCommonConventions();
            });
        }
    }
}