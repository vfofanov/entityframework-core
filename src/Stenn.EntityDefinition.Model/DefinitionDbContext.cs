using Microsoft.EntityFrameworkCore;
using Stenn.EntityDefinition.Tests.Model.Configurations;

namespace Stenn.EntityDefinition.Tests.Model
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
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserRoleMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}