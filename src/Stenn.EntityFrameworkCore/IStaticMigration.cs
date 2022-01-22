namespace Stenn.EntityFrameworkCore
{
    public interface IStaticMigration
    {
        string Name { get; }

        byte[] Hash { get; }

        /// <summary>
        /// Migration's order
        /// </summary>
        int Order => 0;
    }
}