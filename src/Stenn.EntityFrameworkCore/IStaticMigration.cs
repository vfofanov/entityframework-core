namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigration
    {
        string Name { get; }

        byte[] Hash { get; }
    }
}