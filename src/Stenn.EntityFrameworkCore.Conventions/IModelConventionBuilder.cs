using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public interface IModelConventionBuilder
    {
        void Add(IEntityConvention convention);

        void AddInterfaceConvention<TConvention>(Action<EntityTypeBuilder> configure);
        
        void AddInterfaceConventionProperty<TConvention>(Expression<Func<TConvention, object?>> propertyExpression,
            Action<EntityTypeBuilder, PropertyInfo, PropertyBuilder> configure);
    }
}