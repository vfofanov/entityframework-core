using System.Collections.Generic;

namespace Stenn.StaticMigrations
{
    public interface IStaticMigrationCollection<T, TContext> : IEnumerable<StaticMigrationItemFactory<T, TContext>>
    {
        StaticMigrationItemFactory<T, TContext> this[int index] { get; }
        int Count { get; }
    }
}