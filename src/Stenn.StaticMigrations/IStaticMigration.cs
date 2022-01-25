namespace Stenn.StaticMigrations
{
    public interface IStaticMigration
    {
        byte[] Hash { get; }
    }
}