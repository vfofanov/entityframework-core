using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Data.Configurations
{
    public class RoleMap: IEntityTypeConfiguration<Role>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            
            builder.HasKey(x => x.Id);
        }
    }
}