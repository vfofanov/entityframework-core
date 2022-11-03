using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityDefinition.Tests.Model.Configurations
{
    public class UserRoleMap: IEntityTypeConfiguration<UserRole>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(nameof(UserRole.UserId), nameof(UserRole.RoleId));
            builder.HasOne(x => x.User).WithMany(x => x.Roles);
            builder.HasOne(x => x.Role).WithMany(x => x.Users);
        }
    }
}