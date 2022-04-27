using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Stenn.EntityConventions.Contacts;

namespace Stenn.EntityFrameworkCore.EntityConventions
{
    internal sealed class SourceSystemIdValueGenerator : ValueGenerator
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;
        /// <inheritdoc />
        protected override object NextValue(EntityEntry entry)
        {
            return ((IWithSourceSystemIdEntityConvention)entry.Entity).GenerateSourceSystemId();
        }
    }
}