#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
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
            TestSplitted<DbContext0>(ArraySegment<string>.Empty,
                new[] { "00_StartInitialMigration", "01_Migration01" });
        }

        [Test]
        public void CombineSplitted0_00Applied()
        {
            TestSplitted<DbContext0>(new[] { "00_StartInitialMigration" },
                new[] { "01_Migration01" });
        }

        [Test]
        public void CombineSplitted1_Empty()
        {
            TestSplitted<DbContext1>(ArraySegment<string>.Empty,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted1_AppliedTo_00_StartInitialMigration()
        {
            TestSplitted<DbContext1>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted1_AppliedTo_11_Migration11()
        {
            TestSplitted<DbContext1>(new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11"
                },
                new[] { "12_Migration12" });
        }

        [Test]
        public void CombineSplitted2_Empty()
        {
            var actual = TestSplitted<DbContext2>(ArraySegment<string>.Empty,
                new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });

            actual[0].Value.Should().Be<SplittedInitialMigration20>();
        }

        [Test]
        public void CombineSplitted2_AppliedTo_00_StartInitialMigration()
        {
            var actual = TestSplitted<DbContext2>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });

            TestInitialMigrationReplace(actual, 4,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_11_Migration11()
        {
            var actual = TestSplitted<DbContext2>(new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11"
                },
                new[]
                {
                    "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });


            TestInitialMigrationReplace(actual, 1,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_20_SplittedInitialMigration20()
        {
            TestSplitted<DbContext2>(new[] { "20_SplittedInitialMigration20" },
                new[] { "21_Migration21", "22_Migration22" });
        }

        [Test]
        public void CombineSplitted2_AppliedTo_21_Migration21()
        {
            TestSplitted<DbContext2>(new[] { "20_SplittedInitialMigration20", "21_Migration21" },
                new[] { "22_Migration22" });
        }

        [Test]
        public void CombineSplitted3_Empty()
        {
            var actual = TestSplitted<DbContext3>(ArraySegment<string>.Empty,
                new[]
                {
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            actual[0].Value.Should().Be<SplittedInitialMigration31>();
        }

        [Test]
        public void CombineSplitted3_AppliedTo_00_StartInitialMigration()
        {
            var actual = TestSplitted<DbContext3>(new[] { "00_StartInitialMigration" },
                new[]
                {
                    "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12",
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22",
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            TestInitialMigrationReplace(actual, 4,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });

            TestInitialMigrationReplace(actual, 7,
                new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_11_Migration11()
        {
            var actual = TestSplitted<DbContext3>(new[]
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

            TestInitialMigrationReplace(actual, 1,
                new[]
                {
                    "00_StartInitialMigration", "01_Migration01",
                    "10_SplittedMigration10", "11_Migration11", "12_Migration12"
                });

            TestInitialMigrationReplace(actual, 4,
                new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_21_Migration21()
        {
            var actual = TestSplitted<DbContext3>(new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21"
                },
                new[]
                {
                    "22_Migration22",
                    "31_SplittedInitialMigration31", "30_Migration30", "32_Migration32"
                });

            TestInitialMigrationReplace(actual, 1,
                new[]
                {
                    "20_SplittedInitialMigration20", "21_Migration21", "22_Migration22"
                });
        }


        [Test]
        public void CombineSplitted3_AppliedTo_31_SplittedInitialMigration31()
        {
            TestSplitted<DbContext3>(new[] { "31_SplittedInitialMigration31" },
                new[] { "30_Migration30", "32_Migration32" });
        }

        [Test]
        public void CombineSplitted3_AppliedTo_30_Migration30()
        {
            TestSplitted<DbContext3>(new[] { "31_SplittedInitialMigration31", "30_Migration30" },
                new[] { "32_Migration32" });
        }

        private List<KeyValuePair<string, TypeInfo>> TestSplitted<TDbContext>(IEnumerable<string> appliedMigrationEntries, string[] expected)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var sorter = GetMigrationsSorter<TDbContext>();
            var actual = sorter.PopulateMigrations(appliedMigrationEntries).ToList();
            var actualIds = actual.Select(m => m.Key).ToList();

            actualIds.Should().ContainInOrder(expected);

            return actual;
        }
        
        private static void TestInitialMigrationReplace(IReadOnlyList<KeyValuePair<string, TypeInfo>> actual, 
            int index, string[] expectedRemoveIds)
        {
            actual[index].Value.Should().BeAssignableTo<InitialMigrationReplaceBase>();
            actual[index].Value.GetInitialMigration()
                .RemoveMigrationRowIds.Should().ContainInOrder(expectedRemoveIds);
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

        private SplittedMigrationsAssembly GetMigrationsSorter<TContext>()
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var context = GetContext<TContext>(out var options);
            var contextProvider = context.GetInfrastructure();

            var currentDbContext = contextProvider.GetRequiredService<ICurrentDbContext>();

            var idGenerator = contextProvider.GetRequiredService<IMigrationsIdGenerator>();
            var logger = contextProvider.GetRequiredService<IDiagnosticsLogger<DbLoggerCategory.Migrations>>();

            var historyRepository = contextProvider.GetRequiredService<IHistoryRepository>();
            return new SplittedMigrationsAssembly(currentDbContext, options, idGenerator, logger, historyRepository);
        }
    }
}