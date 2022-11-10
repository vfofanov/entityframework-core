using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations.StaticMigrations;
using Stenn.StaticMigrations;
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
        private readonly IStaticMigrationHistoryRepository _staticMigrationHistoryRepository;

        public StaticMigrationCollection<IStaticSqlMigration, Microsoft.EntityFrameworkCore.DbContext> _sqlMigrations { get; set; }

        public StaticMigrationServiceTests()
        {
            _staticMigrationHistoryRepository = new HistoryRepositoryMock();

            _sqlMigrations = new StaticMigrationCollection<IStaticSqlMigration, Microsoft.EntityFrameworkCore.DbContext>();
            _sqlMigrations.Add("migration1", _ => new ResStaticSqlMigration(ResFile.Relative("EmbeddedMigrations\\migration1.sql"), null), null);
            _sqlMigrations.Add("migration2", _ => new ResStaticSqlMigration(ResFile.Relative("EmbeddedMigrations\\migration2.sql"), null), (x) => { return x.ChangedMigrations.Where(i => i.Name.Contains("_Rls")).Any(); });

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

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        ///
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance object.</param>
        /// <param name="fieldName">The field's name which is to be fetched.</param>
        ///
        /// <returns>The field value from the object.</returns>
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
            var result = new List<StaticMigrationHistoryRow>();
            return result;
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
