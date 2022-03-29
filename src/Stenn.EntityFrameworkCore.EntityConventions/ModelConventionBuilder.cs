#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public class ModelConventionBuilder : IModelConventionBuilder
    {
        private readonly List<IEntityConvention> _conventions = new();

        public void Add(IEntityConvention convention)
        {
            if (convention == null)
            {
                throw new ArgumentNullException(nameof(convention));
            }
            _conventions.Add(convention);
        }

        /// <inheritdoc />
        public void AddInterfaceConvention<TConvention>(Action<EntityTypeBuilder> configure)
        {
            Add(new InterfaceConvention(typeof(TConvention), configure));
        }

        public void AddInterfaceConventionProperty<TConvention>(Expression<Func<TConvention, object?>> propertyExpression,
            Action<EntityTypeBuilder, PropertyInfo, PropertyBuilder> configure)
        {
            var propInfo = (PropertyInfo)propertyExpression.GetMemberAccess();
            Add(new InterfaceConvention(typeof(TConvention),
                builder =>
                {
                    var prop = builder.Property(propInfo.PropertyType, propInfo.Name);
                    configure(builder, propInfo, prop);
                }));
        }

        public void Build(ModelBuilder builder)
        {
            //NOTE: Convensions applying to root classes only
            foreach (var entityType in builder.Model.GetEntityTypes().Where(e => e.BaseType is null))
            {
#pragma warning disable EF1001
                var entityBuilder = new EntityTypeBuilder(entityType);
#pragma warning restore EF1001
                foreach (var convention in _conventions.Where(c => c.Allowed(entityType)))
                {
                    convention.Configure(entityBuilder);
                }
            }
        }
    }
}