using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class ResStaticSqlMigration : StaticMigration, IStaticSqlMigration
    {
        private readonly Assembly _assembly;
        private readonly string _applyEmbeddedResFileName;
        private readonly string? _revertEmbeddedResFileName;
        private readonly bool _suppressTransaction;

        public ResStaticSqlMigration(Assembly assembly, string applyEmbeddedResFileName, string? revertEmbeddedResFileName, bool suppressTransaction)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (string.IsNullOrEmpty(applyEmbeddedResFileName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(applyEmbeddedResFileName));
            }

            _assembly = assembly;
            _applyEmbeddedResFileName = applyEmbeddedResFileName;
            _revertEmbeddedResFileName = revertEmbeddedResFileName;
            _suppressTransaction = suppressTransaction;
        }

        protected override byte[] GetHash()
        {
            using var stream = _assembly.ReadResStream(_applyEmbeddedResFileName);
            return HashAlgorithm.ComputeHash(stream);
        }


        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetRevertOperations()
        {
            if (string.IsNullOrEmpty(_revertEmbeddedResFileName))
            {
                yield break;
            }
            var sql = _assembly.ReadRes(_revertEmbeddedResFileName);
            yield return new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction };
        }

        /// <inheritdoc />
        public IEnumerable<MigrationOperation> GetApplyOperations(bool isNew)
        {
            var sql = _assembly.ReadRes(_applyEmbeddedResFileName);
            yield return new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction };
        }
    }
}