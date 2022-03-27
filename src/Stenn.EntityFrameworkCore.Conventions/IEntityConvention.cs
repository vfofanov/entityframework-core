using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public interface IEntityConvention
    {
        bool Allowed(ITypeBase entity);
        void Configure(EntityTypeBuilder builder);
    }
}