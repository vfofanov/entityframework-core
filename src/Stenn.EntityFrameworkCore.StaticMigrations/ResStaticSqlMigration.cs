using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.Shared.Resources;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public sealed class ResStaticSqlMigration : StaticMigration, IStaticSqlMigration
    {
        private readonly ResFile? _applyResFile;
        private readonly ResFile? _revertResFile;

        public ResStaticSqlMigration(ResFile? applyResFile, ResFile? revertResFile)
        {
            if (applyResFile is null && revertResFile is null)
            {
                throw new ArgumentNullException(nameof(applyResFile));
            }
            _applyResFile = applyResFile;
            _revertResFile = revertResFile;
        }

        protected override byte[] GetHashInternal()
        {
            using var stream = (_applyResFile ?? _revertResFile)?.ReadStream() ?? new MemoryStream(new byte[] { 0 });
            return HashAlgorithm.ComputeHash(stream);
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations()
        {
            if (_revertResFile == null)
            {
                yield break;
            }
            var sql = _revertResFile.Read();
            yield return new SqlOperation { Sql = sql, SuppressTransaction = false };
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations()
        {
            if (_applyResFile == null)
            {
                yield break;
            }
            var sql = _applyResFile.Read();
            yield return new SqlOperation { Sql = sql, SuppressTransaction = false };
        }
    }
}