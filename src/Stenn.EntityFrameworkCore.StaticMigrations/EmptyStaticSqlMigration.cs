using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public sealed class EmptyStaticSqlMigration : IStaticSqlMigration
    {
        /// <inheritdoc />
        public byte[] GetHash()
        {
            return new byte[] { };
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations()
        {
            yield break;
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations()
        {
            yield break;
        }
    }
}