namespace Stenn.EntityFrameworkCore.HistoricalMigrations.EF6
{
    public interface IEF6MigrationManager
    {
        string[] MigrationIds { get; }
    }
}