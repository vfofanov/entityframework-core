using System;
using System.Collections.Generic;
using Stenn.DictionaryEntities;

namespace Stenn.StaticMigrations
{
    public sealed class DictionaryEntityMigration<T> : DictionaryEntityMigrationBase<T>
        where T : class
    {
        private readonly Func<List<T>> _getItems;
        private List<T>? _items;

        public DictionaryEntityMigration(Func<List<T>> getItems, IDictionaryEntityInfoContext infoContext)
        {
            _getItems = getItems ?? throw new ArgumentNullException(nameof(getItems));
            InfoContext = infoContext;
        }

        // ReSharper disable once ConstantNullCoalescingCondition
        private List<T> Items => _items ??= _getItems() ?? new List<T>();

        /// <inheritdoc />
        protected override byte[] GetHashInternal()
        {
            return GetHashByInfo(Items);
        }

        /// <inheritdoc />
        protected override List<T> GetActual()
        {
            return Items;
        }
    }
}