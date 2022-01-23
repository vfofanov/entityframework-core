using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.DictionaryEntities.Contracts;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Builder for configure static migrations
    /// </summary>
    public sealed class StaticMigrationBuilder
    {
        private readonly StaticMigrationCollection<IDictionaryEntityMigration> _dictEntityMigrations = new();
        private readonly StaticMigrationCollection<IStaticSqlMigration> _sqlMigrations = new();

        /// <summary>
        ///     Add sql resource static migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="applyEmbeddedResFileName">
        ///     Migration's apply script resource file path.
        ///     It can be absolute path or relative. Relative starts with '/', '\' or '.'
        /// </param>
        /// <param name="revertEmbeddedResFileName">
        ///     Migration's revert script resource file path.
        ///     It can be absolute path or relative. Relative starts with '/', '\' or '.'
        /// </param>
        /// <param name="assembly">Assembly wtih scripts resources. If null calling assembly will be used</param>
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddResSql(string name, string applyEmbeddedResFileName, string? revertEmbeddedResFileName, Assembly? assembly = null,
            bool suppressTransaction = false)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var migration = new ResStaticSqlMigration(assembly,
                PrepareResPath(assembly, applyEmbeddedResFileName)!,
                PrepareResPath(assembly, revertEmbeddedResFileName),
                suppressTransaction);

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
            AddDictionaryEntityFactory(name, _ => migration);
        }

        public void AddDictionaryEntityFactory(string name, Func<DbContext, IDictionaryEntityMigration> migrationFactory)
        {
            _dictEntityMigrations.Add(name, migrationFactory);
        }

        private static string? PrepareResPath(Assembly assembly, string? resPath)
        {
            var assemblyName = assembly.GetName().Name;

            if (resPath == null)
            {
                return resPath;
            }

            resPath = resPath.Replace('\\', '.').Replace('/', '.');
            if (resPath.StartsWith('.'))
            {
                //NOTE: This mean relative path
                return assemblyName + "." + resPath.TrimStart('.');
            }
            return resPath;
        }

        internal void Build(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IStaticSqlMigration>), _sqlMigrations));
            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IDictionaryEntityMigration>), _dictEntityMigrations));
        }
    }
}