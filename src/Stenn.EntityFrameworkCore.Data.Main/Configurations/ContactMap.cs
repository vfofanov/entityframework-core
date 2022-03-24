using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Data.Main.Configurations
{
    public class ContactMap: IEntityTypeConfiguration<Contact>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Type).IsRequired();
        }
    }
}