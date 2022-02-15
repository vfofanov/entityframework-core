using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public sealed class ResStaticSqlMigration : StaticMigration, IStaticSqlMigration
    {
        private readonly ResFile _applyResFile;
        private readonly ResFile? _revertResFile;
        private readonly bool _suppressTransaction;

        public ResStaticSqlMigration(ResFile applyResFile, ResFile? revertResFile, bool suppressTransaction)
        {
            _applyResFile = applyResFile ?? throw new ArgumentNullException(nameof(applyResFile));
            _revertResFile = revertResFile;
            _suppressTransaction = suppressTransaction;
        }

        protected override byte[] GetHashInternal()
        {
            using var stream = _applyResFile.ReadStream();
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
            yield return new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction };
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations(bool isNew)
        {
            var sql = _applyResFile.Read();
            yield return new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction };
        }
    }
}