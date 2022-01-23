using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.DictionaryEntities;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class StaticMigrationServiceFactory : IStaticMigrationServiceFactory
    {
        private readonly IStaticMigrationHistoryRepositoryFactory _historyRepositoryFactory;
        private readonly IDictionaryEntityMigratorFactory _dictionaryEntityMigratorFactory;
        private readonly IStaticMigrationCollection<IStaticSqlMigration> _sqlMigrations;
        private readonly IStaticMigrationCollection<IDictionaryEntityMigration> _entityMigrations;


        public StaticMigrationServiceFactory(IStaticMigrationHistoryRepositoryFactory historyRepositoryFactory,
            IDictionaryEntityMigratorFactory dictionaryEntityMigratorFactory,
            IStaticMigrationCollection<IStaticSqlMigration> migrations,
            IStaticMigrationCollection<IDictionaryEntityMigration> entityMigrations)
        {
            _historyRepositoryFactory = historyRepositoryFactory;
            _dictionaryEntityMigratorFactory = dictionaryEntityMigratorFactory;
            _sqlMigrations = migrations;
            _entityMigrations = entityMigrations;
        }

        /// <inheritdoc />
        public IStaticMigrationService Create(DbContext dbContext, HistoryRepositoryDependencies dependencies)
        {
            var historyRepository = _historyRepositoryFactory.Create(dependencies);
            var migrator = _dictionaryEntityMigratorFactory.Create(dbContext.ToDictionaryEntityContext());
            var sqlMigrations = _sqlMigrations.Select(item => new StaticMigrationItem<IStaticSqlMigration>(item.Name, item.Factory(dbContext))).ToArray();
            var entityMigrations = _entityMigrations.Select(item => new StaticMigrationItem<IDictionaryEntityMigration>(item.Name, item.Factory(dbContext))).ToArray();

            return new StaticMigrationService(historyRepository, dbContext, migrator, sqlMigrations, entityMigrations);
        }
    }
}