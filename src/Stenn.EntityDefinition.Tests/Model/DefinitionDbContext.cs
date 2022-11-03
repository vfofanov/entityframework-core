using Microsoft.EntityFrameworkCore;
using Stenn.EntityDefinition.Tests.Model.Configurations;

namespace Stenn.EntityFrameworkCore.Data.Initial
{
    public class DefinitionDbContext: DbContext
    {
        /// <inheritdoc />
        protected DefinitionDbContext()
        {
        }

        /// <inheritdoc />
        public DefinitionDbContext(DbContextOptions<DefinitionDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}