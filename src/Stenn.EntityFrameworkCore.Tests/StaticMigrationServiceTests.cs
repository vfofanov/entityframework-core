using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;
using Stenn.Shared.Resources;
using Stenn.StaticMigrations;
using Stenn.StaticMigrations.MigrationConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.EntityFrameworkCore.Tests
{
    public class StaticMigrationServiceTests
    {
        private readonly IStaticMigrationsService _staticMigrationsService;
        private readonly HistoryRepositoryMock _staticMigrationHistoryRepository;

        private StaticMigrationCollection<IStaticSqlMigration, Microsoft.EntityFrameworkCore.DbContext> _sqlMigrations { get; set; }

        private static ResStaticSqlMigration migration1 = new ResStaticSqlMigration(ResFile.Relative("EmbeddedMigrations\\migration1.sql"), null);
        private static ResStaticSqlMigration migration2 = new ResStaticSqlMigration(ResFile.Relative("EmbeddedMigrations\\migration2.sql"), null);

        public StaticMigrationServiceTests()
        {
            _staticMigrationHistoryRepository = new HistoryRepositoryMock();

            _sqlMigrations = new StaticMigrationCollection<IStaticSqlMigration, Microsoft.EntityFrameworkCore.DbContext>();
            _sqlMigrations.Add("migration1", _ => migration1, null);
            _sqlMigrations.Add("migration2", _ => migration2, (x) => { return x.ChangedMigrations.Where(i => i.Name.Contains("_Rls")).Any(); });

            _staticMigrationsService = new StaticMigrationsService(_staticMigrationHistoryRepository, new CurrentDbContextMock(), _sqlMigrations);
        }

        [Test]
        public void ServiceShouldGetConditionsFromMigrationsCollection()
        {
            var items = GetInstanceField(typeof(StaticMigrationsService), _staticMigrationsService, "_sqlMigrations") as StaticMigrationItem<IStaticSqlMigration>[];
            items.Count().Should().NotBe(0);
        }

        [Test]
        public void ServiceShouldReturnApplyOperations()
        {
            var ops = _staticMigrationsService.GetApplyOperations(DateTime.Now, false);
            ops.Count().Should().NotBe(0);
        }

        [Test]
        public void ServiceShouldReturnChangedMigraions()
        {
            // adding migration1 to history mock. GetChangedMigrations should return only one migration - migration2
            _staticMigrationHistoryRepository.Rows.Add(new StaticMigrationHistoryRow("migration1", StaticMigrationServiceTests.migration1.GetHash(), DateTime.Now.AddDays(-1)));

            MethodInfo dynMethod = _staticMigrationsService.GetType().GetMethod("GetChangedMigrations", BindingFlags.NonPublic | BindingFlags.Instance);
            List<IStaticMigrationConditionItem> migrations = (List<IStaticMigrationConditionItem>)dynMethod.Invoke(_staticMigrationsService, null);

            migrations.Count().Should().Be(1);
            migrations[0].Name.Should().Be("migration2");
        }

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }

    public class CurrentDbContextMock : ICurrentDbContext
    {
        public Microsoft.EntityFrameworkCore.DbContext Context { get; set; }
    }

    public class HistoryRepositoryMock : IStaticMigrationHistoryRepository
    {
        public List<StaticMigrationHistoryRow> Rows { get; set; } = new List<StaticMigrationHistoryRow>();

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<StaticMigrationHistoryRow> GetAppliedMigrations()
        {
            return Rows;
        }

        public Task<IReadOnlyList<StaticMigrationHistoryRow>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public string GetCreateIfNotExistsScript()
        {
            return "GetCreateIfNotExistsScript";
        }

        public string GetDeleteScript(StaticMigrationHistoryRow row)
        {
            throw new NotImplementedException();
        }

        public string GetInsertScript(StaticMigrationHistoryRow row)
        {
            return "GetInsertScript";
        }
    }
}
