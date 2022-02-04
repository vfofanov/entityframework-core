namespace Stenn.StaticMigrations
{
    public interface IStaticMigration
    {
        byte[] GetHash();
    }
}