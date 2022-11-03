using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.EntityDefinition.Tests.Model;
using Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations.DictEntities;

namespace Stenn.EntityFrameworkCore.Data.Initial.Configurations
{
    public class UserMap: IEntityTypeConfiguration<User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(x => x.Iso3LetterCode).IsUnicode(false).HasMaxLength(3).IsFixedLength().IsRequired();
            builder.HasKey(x => x.Iso3LetterCode);
            
            builder.Property(x => x.IsoNumericCode).IsRequired();
            builder.Property(x => x.DecimalDigits).IsRequired();
            builder.Property(x => x.Description).IsUnicode().HasMaxLength(150);

            builder.HasData(CurrencyDeclaration.GetActual());
        }
    }
}