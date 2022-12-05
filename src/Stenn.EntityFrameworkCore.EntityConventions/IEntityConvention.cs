using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public interface IEntityConvention
    {
#if NET5_0
        bool Allowed(ITypeBase entity);
#else
        bool Allowed(IReadOnlyTypeBase entity);
#endif
        void Configure(EntityTypeBuilder builder);
    }
}