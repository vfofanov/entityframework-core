using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    public class InterfaceConvention : IEntityConvention
    {
        private readonly Type _conventionType;
        private readonly Action<EntityTypeBuilder> _configure;

        public InterfaceConvention(Type conventionType, Action<EntityTypeBuilder> configure)
        {
            _conventionType = conventionType ?? throw new ArgumentNullException(nameof(conventionType));
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

#if NET5_0
        public bool Allowed(ITypeBase entity)
#else
        public bool Allowed(IReadOnlyTypeBase entity)
#endif
        {
            return entity.ClrType.IsAssignableTo(_conventionType);
        }

        public void Configure(EntityTypeBuilder builder)
        {
            _configure(builder);
        }
    }
}