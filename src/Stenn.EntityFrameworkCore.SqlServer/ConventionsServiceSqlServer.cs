using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.EntityFrameworkCore.Conventions;

namespace Stenn.EntityFrameworkCore.SqlServer
{
    public class ConventionsServiceSqlServer : IConventionsService
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder builder)
        {
            foreach (var property in builder.Metadata.GetProperties())
            {
                var propBuilder = builder.Property(property.Name);
                
                if (property.FindAnnotation(ConventionsAnnotationNames.SqlDefault_DateTimeNow) is { })
                {
                    propBuilder.HasDefaultValueSql("getdate()");
                }
                if (property.FindAnnotation(ConventionsAnnotationNames.ColumnTriggerUpdate_SqlDefault) is { })
                {
                    var sqlDefault = property.GetAnnotation(RelationalAnnotationNames.DefaultValueSql);
                    propBuilder.HasAnnotation(ConventionsAnnotationNames.ColumnTriggerUpdate, sqlDefault);
                }
            }
        }
    }
}