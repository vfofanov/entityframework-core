using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute;

namespace Stenn.EntityFrameworkCore.Data.Main.Configurations
{
    public class VCurrencyMap: IEntityTypeConfiguration<VCurrency>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<VCurrency> builder)
        {
            RelationalEntityTypeBuilderExtensions.ToView((EntityTypeBuilder)builder, "vCurrency");
            builder.HasKey(x => x.Iso3LetterCode);
        }
    }
}