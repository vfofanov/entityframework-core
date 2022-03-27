#nullable enable
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Stenn.Conventions.Contacts;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public static class DbContextModelBuilderExtensions
    {
        public static void AddCommonConventions(this IModelConventionBuilder builder)
        {
            builder.AddProperty<ICreateAuditedEntity>(x => x.Created,
                (_, _, p) => p.IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasAnnotation(ConventionsAnnotationNames.SqlDefault_DateTimeNow, true)
                    .HasComment("Row creation datetime. Configured by convention 'ICreateAuditedEntity'"));

            builder.AddProperty<IUpdateAuditedEntity>(x => x.ModifiedAt,
                (_, _, p) => p.IsRequired()
                    .ValueGeneratedOnAddOrUpdate()
                    .HasAnnotation(ConventionsAnnotationNames.SqlDefault_DateTimeNow, true)
                    .HasAnnotation(ConventionsAnnotationNames.ColumnTriggerUpdate_SqlDefault, true)
                    .HasComment("Row last modified datetime. Updated by trigger. Configured by convention 'IUpdateAuditedEntity'"));

            builder.AddProperty<ISoftDeleteEntity>(x => x.Deleted,
                (e, _, p) =>
                {
                    p.IsRequired(false)
                        .HasAnnotation(ConventionsAnnotationNames.ColumnTriggerSoftDelete, true)
                        .HasComment(
                            "Row deleted  datetime. Used for soft delete row. Updated by 'instead of' trigger. Configured by convention 'ISoftDeleteEntity'");

                    e.HasQueryFilter((Expression<Func<object, bool>>)
                        (x => EF.Property<DateTime?>(e, nameof(ISoftDeleteEntity.Deleted)) != null));
                });

            builder.AddProperty<IConcurrentAuditedEntity>(x => x.RowVersion,
                (_, _, p) => p.IsRequired()
                    .IsRowVersion()
                    .HasComment("Concurrent token(row version). Configured by convention 'IConcurrentAuditedEntity'"));


            builder.AddProperty<IManualConcurrentAuditedEntity>(x => x.ManualRowVersion,
                (_, _, p) => p.IsConcurrencyToken()
                    .HasComment("Manual concurrent token. Configured by convention 'IManualConcurrentAuditedEntity'"));

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

        public static void ApplyConventions(this ModelBuilder builder,
            IInfrastructure<IServiceProvider> dbContext,
            Action<IModelConventionBuilder> init)
        {
            var convensionBuilder = new ModelConventionBuilder();

            init(convensionBuilder);

            var serviceProvider = dbContext.GetInfrastructure().GetService<IConventionsService>();
            convensionBuilder.Build(serviceProvider, builder);
        }
    }
}