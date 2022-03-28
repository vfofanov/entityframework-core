#nullable enable
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stenn.Conventions.Contacts;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public static class ModelBuilderConventionsExtensions
    {
        public static void AddCommonConventions(this IModelConventionBuilder builder)
        {
            builder.AddProperty<ICreateAuditedEntity>(x => x.Created,
                (_, _, p) => p.IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasAnnotation(ConventionsAnnotationNames.SqlDefault_CurrentDateTime, true)
                    .HasComment("Row creation datetime. Configured by convention 'ICreateAuditedEntity'"));

            builder.AddProperty<IUpdateAuditedEntity>(x => x.ModifiedAt,
                (_, _, p) => p.IsRequired()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasAnnotation(ConventionsAnnotationNames.SqlDefault_CurrentDateTime, true)
                    .HasAnnotation(ConventionsAnnotationNames.ColumnTriggerUpdate_SqlDefault, true)
                    .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'"));

            builder.AddProperty<ISoftDeleteEntity>(x => x.Deleted,
                (e, _, p) =>
                {
                    p.IsRequired(false)
                        .HasAnnotation(ConventionsAnnotationNames.ColumnTriggerSoftDelete, true)
                        .HasComment(
                            "Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    
                    var entityParam = Expression.Parameter(e.Metadata.ClrType, "x");
                    var deletedPropInfo = typeof(ISoftDeleteEntity).GetProperty(nameof(ISoftDeleteEntity.Deleted))!;
                    var propAccess = Expression.MakeMemberAccess(entityParam, deletedPropInfo);
                    var lambdaExpression = Expression.Lambda(
                        Expression.MakeBinary(ExpressionType.Equal, propAccess, Expression.Constant(null)),
                        entityParam);
                    e.HasQueryFilter(lambdaExpression);
                });

            builder.AddProperty<IConcurrentAuditedEntity>(x => x.RowVersion,
                (_, _, p) => p.IsRequired()
                    .IsRowVersion()
                    .HasComment("Concurrent token(row version). Configured by convention 'IConcurrentAuditedEntity'"));

            builder.AddProperty<IEntityWithSourceSystemId>(x => x.SourceSystemId, (e, i, p) =>
            {
                p.IsRequired()
                    .HasMaxLength(50)
                    .HasValueGenerator<SourceSystemIdValueGenerator>()
                    .HasComment(
                        "Source system id. Row id for cross services' communication. Uses trigger on row insertion. Configured by convention 'IEntityWithSourceSystemId'");

                e.HasIndex(i.Name).IsUnique();
            });
        }

        

        public static void ApplyConventions(this ModelBuilder builder, Action<IModelConventionBuilder> init)
        {
            var convensionBuilder = new ModelConventionBuilder();

            init(convensionBuilder);

            convensionBuilder.Build(builder);
        }
    }
}