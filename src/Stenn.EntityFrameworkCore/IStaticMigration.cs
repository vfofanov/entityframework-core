namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigration
    {
        byte[] Hash { get; }
    }
}