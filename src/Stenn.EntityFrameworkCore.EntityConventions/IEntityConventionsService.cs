using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public interface IEntityConventionsService
    {
        bool HasConventions { get; }
        void Configure(EntityTypeBuilder entityBuilder);
    }
}