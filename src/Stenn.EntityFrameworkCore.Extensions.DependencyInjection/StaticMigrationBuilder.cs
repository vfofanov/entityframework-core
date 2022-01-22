using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    /// <summary>
    ///     Builder for configure static migrations
    /// </summary>
    public sealed class StaticMigrationBuilder
    {
        private readonly List<IDictionaryEntityMigration> _dictEntityMigrations = new();
        private readonly List<IStaticSqlMigration> _sqlMigrations = new();

        /// <summary>
        ///     Add sql resource static migration
        /// </summary>
        /// <param name="name">Migration's name</param>
        /// <param name="applyEmbeddedResFileName">Migration's apply script resource file path.
        /// It can be absolute path or relative. Relative starts with '/', '\' or '.'</param>
        /// <param name="revertEmbeddedResFileName">Migration's revert script resource file path.
        /// It can be absolute path or relative. Relative starts with '/', '\' or '.'</param>
        /// <param name="assembly">Assembly wtih scripts resources. If null calling assembly will be used</param>
        /// <param name="suppressTransaction">Indicates whether or not transactions will be suppressed while executing the SQL</param>
        public void AddResSql(string name, string applyEmbeddedResFileName, string? revertEmbeddedResFileName, Assembly? assembly = null,
            bool suppressTransaction = false)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var migration = new ResStaticSqlMigration(name, assembly,
                PrepareResPath(assembly, applyEmbeddedResFileName)!,
                PrepareResPath(assembly, revertEmbeddedResFileName),
                suppressTransaction);

            _sqlMigrations.Add(migration);
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
            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IStaticSqlMigration>),
                new StaticMigrationCollection<IStaticSqlMigration>(_sqlMigrations)));
            services.Add(new ServiceDescriptor(typeof(IStaticMigrationCollection<IDictionaryEntityMigration>),
                new StaticMigrationCollection<IDictionaryEntityMigration>(_dictEntityMigrations)));
        }
    }
}