using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModelExtensions
    {
        public static bool IsOwned(this IPropertyBase? property)
        {
            return property is INavigation p && p.ForeignKey.IsOwnership && p.TargetEntityType.IsOwned();
        }
    }
}