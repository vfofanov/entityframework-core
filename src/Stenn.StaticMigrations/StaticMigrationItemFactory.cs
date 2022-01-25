using System;

namespace Stenn.StaticMigrations
{
    public record StaticMigrationItemFactory<T, TContext>(string Name, Func<TContext, T> Factory);
}