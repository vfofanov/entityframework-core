using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Stenn.EntityFrameworkCore
{
    [DebuggerDisplay("Count={Count}")]
    public class StaticMigrationCollection<T> : IStaticMigrationCollection<T>
        where T : IStaticMigration
    {
        private readonly List<T> _items;

        public StaticMigrationCollection(List<T> items)
        {
            _items = items;
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public interface IStaticMigrationCollection<out T> : IEnumerable<T>
    {
        T this[int index] { get; }
        int Count { get; }
    }
}