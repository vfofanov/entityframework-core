#nullable enable
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.EntityConventions.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased.SqlServer;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.HistoricalMigrations.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;
using Stenn.StaticMigrations.MigrationConditions;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class ConditionStorage
    {
        public static Func<StaticMigrationConditionOptions, bool>[] ConditionOnName = { (x) => x.ChangedMigrations.Where(i => i.Name.Contains("TestViews")).Any() };
        public static Func<StaticMigrationConditionOptions, bool>[] ConditionOnTag = { (x) => x.ForcedRunActionTags.Any(i => i.Contains("vCurrencyTag")) };
        public static Func<StaticMigrationConditionOptions, bool>[] NegativeConditionOnName = { (x) => x.ChangedMigrations.Where(i => i.Name.Contains("NonExistingMagration")).Any() };
        public static Func<StaticMigrationConditionOptions, bool>[] NegativeConditionOnTag = { (x) => x.ForcedRunActionTags.Any(i => i.Contains("NonExistingTag")) };
    }

    public class ConditionalMigrationsTest
    {
        private const string DBName = "stenn_efcore_tests";
        private InitialDbContext _dbContextInitial = null!;

        private IServiceProvider _serviceProviderInitial = null!;

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        public static Action<StaticMigrationBuilder> BuildInit(Func<StaticMigrationConditionOptions, bool>? condition = null)
        {
            Action<StaticMigrationBuilder> action = (migrations) =>
            {
                migrations.AddInitialSqlResFile("InitDB", suppressTransaction: true, typeof(InitialDbContext).Assembly);

                migrations.AddSqlResFile("TestViews", ResSqlFile.All, typeof(InitialDbContext).Assembly);
                migrations.AddSqlResFile("vCurrency", ResSqlFile.All, typeof(InitialDbContext).Assembly, condition);
            };

            return action;
        }

        [TestCase(null)]
        [TestCaseSource(typeof(ConditionStorage), "ConditionOnName")]
        public async Task MigrationsAfterCreateShouldCreateTableAndView(Func<StaticMigrationConditionOptions, bool>? condition = null)
        {
            InitDbContext(BuildInit(condition), false,
                out _serviceProviderInitial,
                out _dbContextInitial);

            await EnsureCreated(_dbContextInitial);

            await VerifyConditionalMigrationsExecuted();
        }

        [TestCaseSource(typeof(ConditionStorage), "ConditionOnTag")]
        public async Task MigrationsAfterMigrateShouldCreateTableAndView(Func<StaticMigrationConditionOptions, bool>? condition = null)
        {
            InitDbContext(BuildInit(condition), false,
                out _serviceProviderInitial,
                out _dbContextInitial);

            await RunMigrations(_dbContextInitial); // need to run migrations here, calling EnsureCreated does not initialize tags

            await VerifyConditionalMigrationsExecuted();
        }

        [TestCaseSource(typeof(ConditionStorage), "NegativeConditionOnName")]
        [TestCaseSource(typeof(ConditionStorage), "NegativeConditionOnTag")]
        public async Task MigrationsWithNegativeConditionShouldNotCreateView(Func<StaticMigrationConditionOptions, bool>? condition = null)
        {
            InitDbContext(BuildInit(condition), false,
                out _serviceProviderInitial,
                out _dbContextInitial);

            await EnsureCreated(_dbContextInitial);

            await VerifyConditionalMigrationsNotExecuted();
        }

        private async Task VerifyConditionalMigrationsExecuted()
        {
            var actual = await _dbContextInitial.Set<CurrencyV1>().ToListAsync();
            var expected = Data.Initial.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            var actualView = await _dbContextInitial.Set<VCurrency>().ToListAsync();
            actualView.Count.Should().Be(expected.Count);
        }

        private async Task VerifyConditionalMigrationsNotExecuted()
        {
            var actual = await _dbContextInitial.Set<CurrencyV1>().ToListAsync();
            var expected = Data.Initial.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            Action getVCurrency = () => _dbContextInitial.Set<VCurrency>().ToList();

            getVCurrency.Should()
                .Throw<Microsoft.Data.SqlClient.SqlException>()
                .WithMessage("Invalid object name 'vCurrency'.");
        }

        private static void InitDbContext<TContext>(Action<StaticMigrationBuilder> init, bool includeCommonConventions,
            out IServiceProvider serviceProvider,
            out TContext dbContext)
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            serviceProvider = GetServices<TContext>(init, includeCommonConventions);
            dbContext = serviceProvider.GetRequiredService<TContext>();
        }

        private static IServiceProvider GetServices<TDbContext>(Action<StaticMigrationBuilder> init,
            bool includeCommonConventions, string dbName = DBName)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString(dbName);

            services.AddDbContext<TDbContext>(builder =>
                {
                    builder.UseSqlServer(connectionString);
                    builder.UseStaticMigrationsSqlServer(b =>
                    {
                        init.Invoke(b);
                        if (includeCommonConventions)
                        {
                            b.AddTriggerBasedEntityConventionsMigrationSqlServer();
                        }
                    });

                    builder.UseHistoricalMigrations();

                    if (includeCommonConventions)
                    {
                        builder.UseEntityConventionsSqlServer(b => { b.AddTriggerBasedCommonConventions(); });
                    }
                },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services.BuildServiceProvider();
        }

        private static async Task RunMigrations(Microsoft.EntityFrameworkCore.DbContext dbContext, bool deleteDb = true)
        {
            var database = dbContext.Database;
            if (deleteDb)
            {
                await database.EnsureDeletedAsync();
            }
            await database.MigrateAsync();
        }

        private static async Task<bool> EnsureCreated(Microsoft.EntityFrameworkCore.DbContext dbContext, bool deleteDb = true)
        {
            var database = dbContext.Database;
            if (deleteDb)
            {
                await database.EnsureDeletedAsync();
            }
            return await database.EnsureCreatedAsync();
        }
    }
}