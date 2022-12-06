using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModelExtensions
    {
        public static bool IsOwned(this INavigation property)
        {
            return property.ForeignKey.IsOwnership && property.TargetEntityType.IsOwned();
        }
    }
}