using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Builder for configure static migrations
    /// </summary>
    public sealed class StaticMigrationBuilder
    {
        internal StaticMigrationCollection<IDictionaryEntityMigration, DbContext> DictEntityMigrations { get; } = new();
        internal StaticMigrationCollection<IStaticSqlMigration, DbContext> SQLMigrations { get; } = new();

        public void AddResSql(string name, string? applyRelativeResFilePath, string? revertRelativeResFilePath, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var applyResFile = string.IsNullOrEmpty(applyRelativeResFilePath) ? null : ResFile.Relative(applyRelativeResFilePath, assembly);
            var revertResFile = string.IsNullOrEmpty(revertRelativeResFilePath) ? null : ResFile.Relative(revertRelativeResFilePath, assembly);
            AddResSql(name, applyResFile, revertResFile);
        }

        /// <summary>
        ///     Add sql resource static migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="applyFile">
        ///     Migration's apply script resource file path.
        /// </param>
        /// <param name="revertFile">
        ///     Migration's revert script resource file path.
        /// </param>
        public void AddResSql(string name, ResFile? applyFile, ResFile? revertFile)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (applyFile is null && revertFile is null)
            {
                throw new ArgumentNullException(nameof(applyFile));
            }
            var migration = new ResStaticSqlMigration(applyFile, revertFile);
            AddStaticSqlFactory(name, _ => migration);
        }

        /// <summary>
        ///     Add sql resource static migration as initial migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="relativeResFilePath">Migration's apply script resource file path.
        ///     It relative for assembly name. If your assembly's root namespace differs from assembly's name use overload</param>
        /// <param name="assembly">Assembly wtih scripts resources. If null calling assembly will be used
        ///     <see cref="AddInitResSql(string,ResFile,bool)" />
        /// </param>
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddInitResSql(string name, string relativeResFilePath, Assembly? assembly = null, bool suppressTransaction = false)
        {
            if (string.IsNullOrEmpty(relativeResFilePath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(relativeResFilePath));
            }
            assembly ??= Assembly.GetCallingAssembly();

            var resFile = ResFile.Relative(relativeResFilePath, assembly);
            AddInitResSql(name, resFile, suppressTransaction);
        }

        /// <summary>
        ///     Add sql resource static migration as initial migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="applyFile">
        ///     Migration's apply script resource file path.
        /// </param>
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddInitResSql(string name, ResFile applyFile, bool suppressTransaction = false)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            var migration = new InitResStaticSqlMigration(applyFile, suppressTransaction);
            AddStaticSqlFactory(name, _ => migration);
        }

        public void AddStaticSqlFactory(string name, Func<DbContext, IStaticSqlMigration> migrationFactory)
        {
            SQLMigrations.Add(name, migrationFactory);
        }

        public void AddDictionaryEntity<T>(Func<List<T>> getActual)
            where T : class
        {
            AddDictionaryEntity(typeof(T).Name, getActual);
        }

        public void AddDictionaryEntity<T>(string name, Func<List<T>> getActual)
            where T : class
        {
            AddDictionaryEntityMigration(name, context => new DictionaryEntityMigration<T>(getActual, context.ToDictionaryEntityContext()));
        }

        public void AddDictionaryEntityMigration<TMigration>(string name)
            where TMigration : IDictionaryEntityMigration, new()
        {
            AddDictionaryEntityMigration(name, _ => new TMigration());
        }

        public void AddDictionaryEntityMigration(string name, Func<DbContext, IDictionaryEntityMigration> migrationFactory)
        {
            DictEntityMigrations.Add(name, migrationFactory);
        }
    }
}