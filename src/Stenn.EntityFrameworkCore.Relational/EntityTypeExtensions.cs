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
            return entity.FindAnnotation(RelationalAnnotationNames.ViewName)?.Value != null;
        }

        /// <summary>
        /// Is entity type mapped to Table
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsTable(this IEntityType entity)
        {
            return entity.FindAnnotation(RelationalAnnotationNames.TableName)?.Value != null;
        }
    }
}