using System.Security.Cryptography;

namespace Stenn.EntityFrameworkCore
{
    public abstract class StaticMigration : IStaticMigration
    {
        public static readonly HashAlgorithm HashAlgorithm = SHA256.Create();
        private byte[]? _hash;

        /// <inheritdoc />
        public virtual byte[] Hash => _hash ??= GetHash();

        protected abstract byte[] GetHash();
    }
}