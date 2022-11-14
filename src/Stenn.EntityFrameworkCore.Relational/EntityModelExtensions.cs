using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityFrameworkCore.Relational
{
    public static class EntityModelExtensions
    {
        public static string GetFinalColumnName(this IProperty property)
        {
            var identifier = GetIdentifier(property.DeclaringEntityType);
            return property.GetFinalColumnName(identifier);
        }

        public static StoreObjectIdentifier GetIdentifier(this IEntityType entity)
        {
            var viewName = entity.GetViewName();
            var type = !string.IsNullOrEmpty(viewName) ? StoreObjectType.View : StoreObjectType.Table;
            return StoreObjectIdentifier.Create(entity, type)!.Value;
        }

        public static string GetFinalColumnName(this IProperty property, StoreObjectIdentifier identifier)
        {
            return property.GetColumnName(identifier) ?? property.GetColumnBaseName();
        }


    }
}