namespace Stenn.EntityFrameworkCore
{
    public record StaticMigrationItem<T>(string Name, T Migration);
}