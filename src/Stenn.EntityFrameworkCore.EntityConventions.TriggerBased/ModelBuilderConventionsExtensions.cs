using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stenn.EntityConventions.Contacts.TriggerBased;

namespace Stenn.EntityFrameworkCore.EntityConventions.TriggerBased
{
    public static class ModelBuilderTriggerBasedConventionsExtensions
    {
        public static void AddTriggerBasedCommonConventions(this IEntityConventionsBuilder builder)
        {
            builder
                .AddUpdateAudited()
                .AddSoftDelete();
        }
        
        public static IEntityConventionsBuilder AddUpdateAudited(this IEntityConventionsBuilder builder)
        {
            builder.AddInterfaceConventionProperty<IUpdateAuditedEntityConvention>(x => x.ModifiedAt,
                (_, _, p) => p.IsRequired()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasAnnotation(ConventionsAnnotationNames.SqlDefault_CurrentDateTime, true)
                    .HasAnnotation(ConventionsAnnotationNames.ColumnTriggerUpdate_SqlDefault, true)
                    .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'"));

            return builder;
        }

        public static IEntityConventionsBuilder AddSoftDelete(this IEntityConventionsBuilder builder)
        {
            builder.AddInterfaceConvention<ISoftDeleteEntityConvention>(
                e =>
                {
                    e.HasAnnotation(ConventionsAnnotationNames.SoftDelete, true);

                    e.Property(typeof(bool), nameof(ISoftDeleteEntityConvention.IsDeleted))
                        .IsRequired()
                        .HasDefaultValue(false)
                        .HasComment(
                            "Row deleted flag. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    e.Property(typeof(DateTime?), nameof(ISoftDeleteEntityConvention.Deleted))
                        .IsRequired(false)
                        .HasComment(
                            "Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    e.HasIndex(nameof(ISoftDeleteEntityConvention.IsDeleted));

                    var entityParam = Expression.Parameter(e.Metadata.ClrType, "x");
                    var deletedPropInfo = typeof(ISoftDeleteEntityConvention).GetProperty(nameof(ISoftDeleteEntityConvention.IsDeleted))!;
                    var propAccess = Expression.MakeMemberAccess(entityParam, deletedPropInfo);
                    var lambdaExpression = Expression.Lambda(
                        Expression.MakeBinary(ExpressionType.Equal, propAccess, Expression.Constant(false)),
                        entityParam);
                    e.HasQueryFilter(lambdaExpression);
                });

            return builder;
        }
        
    }
}