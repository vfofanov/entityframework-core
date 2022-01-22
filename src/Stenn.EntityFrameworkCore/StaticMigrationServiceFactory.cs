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
        private readonly IStaticSqlMigration[] _sqlMigrations;
        private readonly IDictionaryEntityMigration[] _entityMigrations;


        public StaticMigrationServiceFactory(IStaticMigrationHistoryRepositoryFactory historyRepositoryFactory, 
            IEnumerable<IStaticSqlMigration> migrations, IEnumerable<IDictionaryEntityMigration> entityMigrations)
        {
            _historyRepositoryFactory = historyRepositoryFactory;
            _sqlMigrations = migrations.OrderBy(m => m.Order).ToArray();
            _entityMigrations = entityMigrations.OrderBy(m => m.Order).ToArray();
        }

        /// <inheritdoc />
        public IStaticMigrationService Create(DbContext dbContext, HistoryRepositoryDependencies dependencies)
        {
            var historyRepository = _historyRepositoryFactory.Create(dependencies);
            return new StaticMigrationService(historyRepository,dbContext, _sqlMigrations, _entityMigrations);
        }
    }
}