#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.SplittedMigrations;
using Stenn.EntityFrameworkCore.Tests.SplittedMigrations;

namespace Stenn.EntityFrameworkCore.Tests
{
    public class SplittedMigrationsTests
    {
        [Test]
        public void CombineSplitted0_Empty()
        {
            TestSorter<DbContext0>(ArraySegment<string>.Empty,
                new[] { "00_StartInitialMigration", "01_Migration01" });
        }

        [Test]
        public void CombineSplitted0_00Applied()
        {
            TestSorter<DbContext0>(new[] { "00_StartInitialMigration" },
                new[] { "01_Migration01" });
        }

        [Test]
        public void CombineSplitted1_Empty()
        {
            TestSorter<DbContext1>(ArraySegment<string>.Empty,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted1_AppliedTo_00_StartInitialMigration()
        {
            TestSorter<DbContext1>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted1_AppliedTo_11_Migration11()
        {
            TestSorter<DbContext1>(new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11"
                },
                new[] { "12_Migration12" });
        }

        [Test]
        public void CombineSplitted2_Empty()
        {
            var actual = TestSorter<DbContext2>(ArraySegment<string>.Empty,
                new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });

            actual[0].Should().BeOfType<SplittedInitialMigration20>();
        }

        [Test]
        public void CombineSplitted2_AppliedTo_00_StartInitialMigration()
        {
            var actual = TestSorter<DbContext2>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });

            actual[4].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "00_StartInitialMigration", "01_Migration01",
                        "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                    });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_11_Migration11()
        {
            var actual = TestSorter<DbContext2>(new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11"
                },
                new[]
                {
                    "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });

            actual[1].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "00_StartInitialMigration", "01_Migration01",
                        "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                    });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_20_SplittedInitialMigration20()
        {
            TestSorter<DbContext2>(new[] { "20_SplittedInitialMigration20" },
                new[] { "21_Migration21", "22_Migration22" });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_21_Migration21()
        {
            TestSorter<DbContext2>(new[] { "20_SplittedInitialMigration20", "21_Migration21" },
                new[] { "22_Migration22" });
        }

        [Test]
        public void CombineSplitted3_Empty()
        {
            var actual = TestSorter<DbContext3>(ArraySegment<string>.Empty,
                new[]
                {
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            actual[0].Should().BeOfType<SplittedInitialMigration31>();
        }

        [Test]
        public void CombineSplitted3_AppliedTo_00_StartInitialMigration()
        {
            var actual = TestSorter<DbContext3>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22",
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            actual[4].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "00_StartInitialMigration", "01_Migration01",
                        "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                    });
            
            actual[7].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                    });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_11_Migration11()
        {
            var actual = TestSorter<DbContext3>(new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11"
                },
                new[]
                {
                    "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22",
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            actual[1].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "00_StartInitialMigration", "01_Migration01",
                        "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                    });
            
            actual[4].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                    });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_21_Migration21()
        {
            var actual = TestSorter<DbContext3>(new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21"
                },
                new[]
                {
                    "22_Migration22",
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            actual[1].Should().BeAssignableTo<InitialMigrationBase>()
                .Subject.RemoveMigrationRowIds.Should().ContainInOrder(
                    new[]
                    {
                        "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                    });
        }
        
        
        [Test]
        public void CombineSplitted3_AppliedTo_31_SplittedInitialMigration31()
        {
            TestSorter<DbContext3>(new[] { "31_SplittedInitialMigration31" },
                new[] { "30_Migration30", "32_Migration32" });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_30_Migration30()
        {
            TestSorter<DbContext3>(new[] { "31_SplittedInitialMigration31", "30_Migration30" },
                new[] { "32_Migration32" });
        }

        private List<Migration> TestSorter<TDbContext>(IEnumerable<string> appliedMigrationEntries, string[] expected)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var sorter = GetMigrationsSorter<TDbContext>();
            var actual = sorter.PopulateMigrations(appliedMigrationEntries).ToList();
            var actualIds = actual.Select(m => m.GetId()).ToList();

            actualIds.Should().ContainInOrder(expected);

            return actual;
        }


        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        private static TDbContext GetContext<TDbContext>(out DbContextOptions options)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString("test_splitted");

            services.AddDbContext<TDbContext>(builder => { builder.UseSqlServer(connectionString); },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            var provider = services.BuildServiceProvider();
            options = provider.GetRequiredService<DbContextOptions<TDbContext>>();
            return provider.GetRequiredService<TDbContext>();
        }

        private MigrationsSorter GetMigrationsSorter<TContext>()
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var context = GetContext<TContext>(out var options);
            var contextProvider = context.GetInfrastructure();

            var currentDbContext = contextProvider.GetRequiredService<ICurrentDbContext>();

            var idGenerator = contextProvider.GetRequiredService<IMigrationsIdGenerator>();
            var logger = contextProvider.GetRequiredService<IDiagnosticsLogger<DbLoggerCategory.Migrations>>();

            var migrationsAssembly = new MigrationsAssembly(currentDbContext, options, idGenerator, logger);

            var historyRepository = contextProvider.GetRequiredService<IHistoryRepository>();
            var databaseProvider = contextProvider.GetRequiredService<IDatabaseProvider>();
            return new MigrationsSorter(migrationsAssembly, historyRepository, currentDbContext, options, idGenerator, logger, databaseProvider);
        }
    }
}