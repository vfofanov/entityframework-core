using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.Data.Initial.Configurations;

namespace Stenn.EntityFrameworkCore.Data.Initial
{
    public class InitialDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        /// <inheritdoc />
        protected InitialDbContext()
        {
        }

        /// <inheritdoc />
        public InitialDbContext(DbContextOptions<InitialDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CurrencyV1Map());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}