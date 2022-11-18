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
            return entity.GetEntityType() == EntityType.View;
        }

        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsSqlQuery(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityType.SqlQuery;
        }
        
        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsInMemoryQuery(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityType.InMemoryQuery;
        }

        /// <summary>
        /// Is entity type mapped to Sql Query
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsFunction(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityType.Function;
        }

        /// <summary>
        /// Is entity type mapped to Table
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsTable(this IEntityType entity)
        {
            return entity.GetEntityType() == EntityType.Table;
        }

        public static bool IsComputed(this IProperty property)
        {
            return property.FindAnnotation(RelationalAnnotationNames.ComputedColumnSql) != null;
        }

        public static EntityType GetEntityType(this IEntityType entity)
        {
            if (entity.FindAnnotation(RelationalAnnotationNames.TableName)?.Value != null ||
                entity.FindAnnotation(RelationalAnnotationNames.TableMappings) != null)
            {
                return EntityType.Table;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.ViewName)?.Value != null ||
                entity.FindAnnotation(RelationalAnnotationNames.ViewMappings) != null||
                entity.FindAnnotation(RelationalAnnotationNames.ViewDefinitionSql) != null)
            {
                return EntityType.View;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.FunctionName)?.Value != null ||
                entity.FindAnnotation(RelationalAnnotationNames.FunctionMappings) != null)
            {
                return EntityType.Function;
            }
            if (entity.FindAnnotation(RelationalAnnotationNames.SqlQuery) != null)
            {
                return EntityType.SqlQuery;
            }
#pragma warning disable CS0612
            if (entity.FindAnnotation(CoreAnnotationNames.DefiningQuery) != null)
#pragma warning restore CS0612
            {
                return EntityType.InMemoryQuery;
            }

            if (entity.BaseType is { } baseType)
            {
                return baseType.GetEntityType();
            }
            return EntityType.Table;
        }
    }

    public enum EntityType
    {
        Table = 0,
        View = 1,
        Function = 2,
        SqlQuery = 3,
        InMemoryQuery = 4
    }
}