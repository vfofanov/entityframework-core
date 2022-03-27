using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Conventions
{
    /// <summary>
    /// Db specific service for entity convention
    /// </summary>
    public interface IConventionsService
    {
        void Configure(EntityTypeBuilder builder);
    }
}