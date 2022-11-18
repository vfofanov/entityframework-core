using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            return entity.GetEntityType() == EntityMappingType.View;
        }

        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsSqlQuery(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityMappingType.SqlQuery;
        }
        
        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsInMemoryQuery(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityMappingType.InMemoryQuery;
        }

        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsFunction(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityMappingType.Function;
        }

        /// <summary>
        /// Is entity type mapped to Table
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsTable(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityMappingType.Table;
        }

        public static bool IsComputed(this IProperty property)
        {
            return property.FindAnnotation(RelationalAnnotationNames.ComputedColumnSql) != null;
        }

        public static EntityMappingType GetEntityType(this IEntityType entity)
        {
            if (entity.FindAnnotation(RelationalAnnotationNames.TableName) != null ||
                entity.FindAnnotation(RelationalAnnotationNames.TableMappings) != null)
            {
                return EntityMappingType.Table;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.ViewName) != null ||
                entity.FindAnnotation(RelationalAnnotationNames.ViewMappings) != null)
            {
                return EntityMappingType.View;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.FunctionName) != null ||
                entity.FindAnnotation(RelationalAnnotationNames.FunctionMappings) != null)
            {
                return EntityMappingType.Function;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.SqlQuery) != null)
            {
                return EntityMappingType.SqlQuery;
            }
#pragma warning disable CS0612
            if (entity.FindAnnotation(CoreAnnotationNames.DefiningQuery) != null)
#pragma warning restore CS0612
            {
                return EntityMappingType.InMemoryQuery;
            }

            if (entity.BaseType is { } baseType)
            {
                return baseType.GetEntityType();
            }
            return EntityMappingType.Table;
        }
    }
}