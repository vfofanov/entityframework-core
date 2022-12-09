using Stenn.StaticMigrations.MigrationConditions;
using System;
using System.Collections.Immutable;

namespace Stenn.StaticMigrations
{
    public record StaticMigrationItemFactory<T, TContext>(string Name, Func<TContext, T> Factory,IImmutableSet<string> Tags, Func<StaticMigrationConditionOptions, bool>? Condition);
}