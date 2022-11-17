using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityFrameworkCore.Relational
{
    public static class EntityTypeExtensions
    {
        /// <summary>
        /// Is entity type mapped to View
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsView(this IEntityType entity)
        {
            return entity.FindAnnotation(RelationalAnnotationNames.ViewMappings) != null;
        }

        /// <summary>
        /// Is entity type mapped to Table
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsTable(this IEntityType entity)
        {
            return entity.FindAnnotation(RelationalAnnotationNames.TableMappings) != null;
        }

        public static bool IsComputed(this IProperty property)
        {
            return property.FindAnnotation(RelationalAnnotationNames.ComputedColumnSql) != null;
        }
    }
}