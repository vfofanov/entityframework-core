using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Stenn.EntityFrameworkCore
{
    [DebuggerDisplay("{Name}")]
    public sealed class ResStaticSqlMigration : IStaticSqlMigration
    {
        private static readonly HashAlgorithm HashAlgorithm = SHA256.Create();
        private readonly Assembly _assembly;
        private readonly string _applyEmbeddedResFileName;
        private readonly string? _revertEmbeddedResFileName;
        private readonly bool _suppressTransaction;
        private readonly Lazy<byte[]> _hash;

        public ResStaticSqlMigration(string name, Assembly assembly, string applyEmbeddedResFileName, string? revertEmbeddedResFileName,
            bool suppressTransaction)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (string.IsNullOrEmpty(applyEmbeddedResFileName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(applyEmbeddedResFileName));
            }

            Name = name;

            _assembly = assembly;
            _applyEmbeddedResFileName = applyEmbeddedResFileName;
            _revertEmbeddedResFileName = revertEmbeddedResFileName;
            _suppressTransaction = suppressTransaction;

            _hash = new Lazy<byte[]>(GetHash);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public byte[] Hash => _hash.Value;
        private byte[] GetHash()
        {
            using var stream = _assembly.ReadResStream(_applyEmbeddedResFileName);
            return HashAlgorithm.ComputeHash(stream);
        }
        
        
        /// <inheritdoc />
        public bool FillRevertOperations(List<MigrationOperation> operations)
        {
            if (string.IsNullOrEmpty(_revertEmbeddedResFileName))
            {
                return false;
            }
            var sql = _assembly.ReadRes(_revertEmbeddedResFileName);

            operations.Add(new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction });

            return true;
        }

        /// <inheritdoc />
        public void FillApplyOperations(List<MigrationOperation> operations, bool isNew)
        {
            var sql = _assembly.ReadRes(_applyEmbeddedResFileName);
            operations.Add(new SqlOperation { Sql = sql, SuppressTransaction = _suppressTransaction });
        }
    }
}