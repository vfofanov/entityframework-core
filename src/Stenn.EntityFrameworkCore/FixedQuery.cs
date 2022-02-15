using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore
{
    /// <summary>
    /// Use this class for return empty or fixed items' collection as <see cref="IQueryable{T}"/> for use with
    /// <see cref="EntityFrameworkQueryableExtensions.ToListAsync{TSource}"/> or <see cref="EntityFrameworkQueryableExtensions.ToArrayAsync{TSource}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FixedQuery<T> : IAsyncEnumerable<T>, IQueryable<T>
    {
        /// <summary>
        /// Empty <see cref="IQueryable{T}"/>
        /// </summary>
        public static readonly IQueryable<T> Empty = Create(ArraySegment<T>.Empty);

        /// <summary>
        /// Create  <see cref="IQueryable{T}"/> with fixed items collection
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IQueryable<T> Create(params T[] items)
        {
            return Create((IEnumerable<T>)items);
        }

        /// <summary>
        /// Create  <see cref="IQueryable{T}"/> with fixed items collection
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IQueryable<T> Create(IEnumerable<T>? items)
        {
            return new FixedQuery<T>(items ?? ArraySegment<T>.Empty).AsQueryable();
        }

        private readonly IQueryable<T> _items;

        private FixedQuery(IEnumerable<T> items)
        {
            _items = (items ?? throw new ArgumentNullException(nameof(items))).AsQueryable();
        }

        /// <inheritdoc />
#pragma warning disable CS1998
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
#pragma warning restore CS1998
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

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

        /// <inheritdoc />
        public Type ElementType => _items.ElementType;

        /// <inheritdoc />
        public Expression Expression => _items.Expression;

        /// <inheritdoc />
        public IQueryProvider Provider => _items.Provider;
    }
}