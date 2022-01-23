using System;
using System.Collections.Generic;
using System.Text.Json;
using Stenn.DictionaryEntities.Contracts;

namespace Stenn.EntityFrameworkCore
{
    public sealed class DictionaryEntityMigration<T> : DictionaryEntityMigrationBase<T>
        where T : class, IDictionaryEntity<T>
    {
        private readonly Func<List<T>> _getItems;
        private List<T>? _items;

        public DictionaryEntityMigration(Func<List<T>> getItems)
        {
            _getItems = getItems ?? throw new ArgumentNullException(nameof(getItems));
        }

        private List<T> Items => _items ??= _getItems();

        /// <inheritdoc />
        protected override byte[] GetHash()
        {
            var itemsArray = JsonSerializer.SerializeToUtf8Bytes(Items);
            return HashAlgorithm.ComputeHash(itemsArray);
        }

        /// <inheritdoc />
        protected override List<T> GetActual()
        {
            return Items;
        }
    }
}