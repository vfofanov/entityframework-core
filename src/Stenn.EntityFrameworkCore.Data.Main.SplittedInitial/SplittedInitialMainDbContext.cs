using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Data.Main.Configurations;

namespace Stenn.EntityFrameworkCore.Data.Main.SplittedInitial
{
    public class SplittedInitialMainDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        /// <inheritdoc />
        protected SplittedInitialMainDbContext()
        {
        }

        /// <inheritdoc />
        public SplittedInitialMainDbContext(DbContextOptions<SplittedInitialMainDbContext> options)
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