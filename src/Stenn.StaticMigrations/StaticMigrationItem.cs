namespace Stenn.StaticMigrations
{
    public record StaticMigrationItem<T>(string Name, T Migration) : IStaticMigration
        where T : IStaticMigration
    {
        public byte[] GetHash() => Migration.GetHash();
    }
}