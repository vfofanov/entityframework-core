using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Stenn.EntityDefinition.Contracts.Definitions
{
    public sealed class DefinitionContext : IDisposable
    {
        private readonly ConcurrentDictionary<DefinitionInfo, object> _dict = new();

        public T GetOrAdd<T>(DefinitionInfo info, Func<T> valueFactory)
        {
            return (T)_dict.GetOrAdd(info, _ => valueFactory() ?? throw new NullReferenceException());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var value in _dict.Values.OfType<IDisposable>())
            {
                value.Dispose();
            }
        }
    }
}