using System;
using Microsoft.EntityFrameworkCore;

namespace Stenn.EntityFrameworkCore
{
    public record StaticMigrationItemFactory<T>(string Name, Func<DbContext, T> Factory);
}