using Microsoft.EntityFrameworkCore;
using Stenn.EntityDefinition.Model.Configurations;

namespace Stenn.EntityDefinition.Model
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
            
            modelBuilder.ApplyConfiguration(new InvoiceMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}