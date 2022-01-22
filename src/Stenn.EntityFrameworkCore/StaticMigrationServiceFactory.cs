using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore
{
    public sealed class StaticMigrationServiceFactory : IStaticMigrationServiceFactory
    {
        private readonly IStaticMigrationHistoryRepositoryFactory _historyRepositoryFactory;
        private readonly IStaticMigrationCollection<IStaticSqlMigration> _sqlMigrations;
        private readonly IStaticMigrationCollection<IDictionaryEntityMigration> _entityMigrations;


        public StaticMigrationServiceFactory(IStaticMigrationHistoryRepositoryFactory historyRepositoryFactory, 
            IStaticMigrationCollection<IStaticSqlMigration> migrations, 
            IStaticMigrationCollection<IDictionaryEntityMigration> entityMigrations)
        {
            _historyRepositoryFactory = historyRepositoryFactory;
            _sqlMigrations = migrations;
            _entityMigrations = entityMigrations;
        }

        /// <inheritdoc />
        public IStaticMigrationService Create(DbContext dbContext, HistoryRepositoryDependencies dependencies)
        {
            var historyRepository = _historyRepositoryFactory.Create(dependencies);
            return new StaticMigrationService(historyRepository,dbContext, _sqlMigrations, _entityMigrations);
        }
    }
}