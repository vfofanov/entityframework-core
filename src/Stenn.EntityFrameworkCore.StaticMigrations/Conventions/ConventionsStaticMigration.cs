#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Conventions
{
    public abstract class ConventionsStaticMigration : StaticMigration, IStaticSqlMigration
    {
        public const string NameSuffix = "_#convention#";
        protected readonly DbContext Context;

        public ConventionsStaticMigration(DbContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override byte[] GetHashInternal()
        {
            var items = GetApplyOperations();
            return GetHash(items);
        }

        /// <inheritdoc />
        public abstract IEnumerable<MigrationOperation> GetRevertOperations();

        /// <inheritdoc />
        public virtual IEnumerable<MigrationOperation> GetApplyOperations()
        {
            return Context.Model.GetEntityTypes().Where(e => e.BaseType is null).OrderBy(e => e.Name)
                .SelectMany(GetApplyOperations);
        }

        protected abstract IEnumerable<MigrationOperation> GetApplyOperations(IEntityType entity);
    }
}