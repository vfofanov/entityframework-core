using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.DictionaryEntities;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Builder for configure static migrations
    /// </summary>
    public sealed class StaticMigrationBuilder
    {
        private readonly StaticMigrationCollection<IDictionaryEntityMigration, DbContext> _dictEntityMigrations = new();
        private readonly StaticMigrationCollection<IStaticSqlMigration, DbContext> _sqlMigrations = new();
        private Action<IServiceCollection>? _replaceServices;

        /// <summary>
        ///     Add sql resource static migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="applyRelativeResFilePath">
        ///     Migration's apply script resource file path.
        ///     It relative for assembly name. If your assembly's root namespace differs from assembly's name use overload
        ///     <see cref="AddResSql(string,ResFile,ResFile?,bool)" />
        /// </param>
        /// <param name="revertRelativeResFilePath">
        ///     Migration's revert script resource file path.
        ///     It relative for assembly name. If your assembly's root namespace differs from assembly's name use overload
        ///     <see cref="AddResSql(string,ResFile,ResFile?,bool)" />
        /// </param>
        /// <param name="assembly">Assembly wtih scripts resources. If null calling assembly will be used</param>
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddResSql(string name, string applyRelativeResFilePath, string? revertRelativeResFilePath, Assembly? assembly = null,
            bool suppressTransaction = false)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var applyResFile = ResFile.Relative(applyRelativeResFilePath, assembly);
            var revertResFile = string.IsNullOrEmpty(revertRelativeResFilePath) ? null : ResFile.Relative(revertRelativeResFilePath, assembly);
            AddResSql(name, applyResFile, revertResFile, suppressTransaction);
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
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddResSql(string name, ResFile applyFile, ResFile? revertFile, bool suppressTransaction = false)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (applyFile == null)
            {
                throw new ArgumentNullException(nameof(applyFile));
            }
            var migration = new ResStaticSqlMigration(applyFile, revertFile, suppressTransaction);
            AddStaticSqlFactory(name, _ => migration);
        }

        public void AddStaticSqlFactory(string name, Func<DbContext, IStaticSqlMigration> migrationFactory)
        {
            _sqlMigrations.Add(name, migrationFactory);
        }

        public void AddDictionaryEntity<T>(Func<List<T>> getActual)
            where T : class, IDictionaryEntity<T>
        {
            AddDictionaryEntity(typeof(T).Name, getActual);
        }

        public void AddDictionaryEntity<T>(string name, Func<List<T>> getActual)
            where T : class, IDictionaryEntity<T>
        {
            var migration = new DictionaryEntityMigration<T>(getActual);
            AddDictionaryEntityMigration(name, _ => migration);
        }

        public void AddDictionaryEntityMigration<TMigration>(string name)
            where TMigration : IDictionaryEntityMigration, new()
        {
            AddDictionaryEntityMigration(name, _ => new TMigration());
        }

        public void AddDictionaryEntityMigration(string name, Func<DbContext, IDictionaryEntityMigration> migrationFactory)
        {
            _dictEntityMigrations.Add(name, migrationFactory);
        }

        /// <summary>
        /// Replace static migrations services
        /// </summary>
        /// <param name="replaceServices"></param>
        public void ReplaceServices(Action<IServiceCollection> replaceServices)
        {
            _replaceServices = replaceServices;
        }

        internal void Build(IServiceCollection services)
        {
            _replaceServices?.Invoke(services);

            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IStaticSqlMigration, DbContext>), _sqlMigrations));
            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IDictionaryEntityMigration, DbContext>), _dictEntityMigrations));
        }
    }
}