using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityDefinition.Model.Configurations
{
    public class InvoiceViewExtendedMap : IEntityTypeConfiguration<InvoiceViewExtended>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<InvoiceViewExtended> builder)
        {
            builder.HasBaseType<InvoiceView>();
        }
    }
}