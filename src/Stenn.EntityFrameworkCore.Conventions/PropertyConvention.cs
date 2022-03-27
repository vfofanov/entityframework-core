using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.Conventions
{
    public class PropertyConvention : IEntityConvention
    {
        private readonly Type _conventionType;
        private readonly PropertyInfo _propertyInfo;
        private readonly Action<EntityTypeBuilder,PropertyInfo, PropertyBuilder> _configure;

        public PropertyConvention(Type conventionType, PropertyInfo propertyInfo, Action<EntityTypeBuilder,PropertyInfo, PropertyBuilder> configure)
        {
            _conventionType = conventionType ?? throw new ArgumentNullException(nameof(conventionType));
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public bool Allowed(ITypeBase entity)
        {
            return entity.ClrType.IsAssignableTo(_conventionType);
        }

        public void Configure(EntityTypeBuilder builder)
        {
            var prop = builder.Property(_propertyInfo.PropertyType, _propertyInfo.Name);
            _configure(builder, _propertyInfo, prop);
        }
    }
}