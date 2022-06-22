using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations.DictEntities;

namespace Stenn.EntityFrameworkCore.Data.Initial.Configurations
{
    public class VCurrencyMap: IEntityTypeConfiguration<VCurrency>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<VCurrency> builder)
        {
            builder.ToView("vCurrency");
            builder.HasKey(x => x.Iso3LetterCode);
        }
    }
}