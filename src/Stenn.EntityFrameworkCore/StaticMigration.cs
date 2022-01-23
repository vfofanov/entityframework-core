using System.Security.Cryptography;

namespace Stenn.EntityFrameworkCore
{
    public abstract class StaticMigration : IStaticMigration
    {
        public static readonly HashAlgorithm HashAlgorithm = SHA256.Create();
        protected StaticMigration(string name)
        {
            Name = name;
        }
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public abstract byte[] Hash { get; }
    }
}