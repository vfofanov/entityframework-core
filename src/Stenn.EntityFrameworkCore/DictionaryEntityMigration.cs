using System;
using System.Threading;
using System.Threading.Tasks;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore
{
    public sealed class DictionaryEntityMigration<T> : IDictionaryEntityMigration
    {
        protected DictionaryEntityMigrationBase(string name, Func<T> getItems)
        {
            Name = name;
            Hash = hash;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public byte[] Hash { get; }

        /// <inheritdoc />
        public Task Update(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            return null;
        }

        /// <inheritdoc />
        public Task UpdateAsync(IDictionaryEntityMigrator migrator, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}