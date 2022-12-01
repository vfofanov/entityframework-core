using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Data.Main.Migrations.Static;
using Stenn.EntityFrameworkCore.EntityConventions.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased.SqlServer;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.HistoricalMigrations.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Main = Stenn.EntityFrameworkCore.Data.Main;
using MainWithoutAttribute = Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class HistoricalMigrationsRegistrationTests
    {
        private const string DBName = "stenn_efcore_historic_migrations_tests";

        private Main.MainDbContext _dbContextMain = null!;
        private MainWithoutAttribute.MainDbContext _dbContextMainHistoricalWithoutAttribute = null!;
        private Main.MainDbContext _dbContextMainTwoRegistrations = null!;

        private IServiceProvider _serviceProviderMain = null!;
        private IServiceProvider _serviceProviderMainHistoricalWithoutAttribute = null!;
        private IServiceProvider _serviceProviderMainTwoRegistrations = null!;

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        [SetUp]
        public void Setup()
        {
            InitDbContext(MainStaticMigrations.Init,
                false,
                true,
                out _serviceProviderMain,
                out _dbContextMain);

            InitDbContext(MainStaticMigrations.Init,
                true,
                true,
                out _serviceProviderMainHistoricalWithoutAttribute,
                out _dbContextMainHistoricalWithoutAttribute);

            InitDbContext(MainStaticMigrations.Init,
                true,
                true,
                out _serviceProviderMainTwoRegistrations,
                out _dbContextMainTwoRegistrations);
        }

        private static void InitDbContext<TContext>(Action<StaticMigrationBuilder> init, bool useGenericRegistration, bool includeCommonConventions,
            out IServiceProvider serviceProvider,
            out TContext dbContext)
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            serviceProvider = GetServices<TContext>(init, useGenericRegistration, includeCommonConventions);
            dbContext = serviceProvider.GetRequiredService<TContext>();
        }

        private static IServiceProvider GetServices<TDbContext>(Action<StaticMigrationBuilder> init, bool useGenericRegistration,
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

                if (useGenericRegistration)
                {
                    builder.UseHistoricalMigrations<MainDbContext_Step1PlusStep2>();
                }
                else
                {
                    builder.UseHistoricalMigrations();
                }

                if (includeCommonConventions)
                {
                    builder.UseEntityConventionsSqlServer(b => { b.AddTriggerBasedCommonConventions(); });
                }
            },
            ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services.BuildServiceProvider();
        }

        [Test]
        public async Task EnsureCreated_Main()
        {
            await EnsureCreated(_dbContextMain);

            var actual = await _dbContextMain.Set<Main.Currency>().ToListAsync();
            var expected = Main.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            var actualRoles = await _dbContextMain.Set<Main.Role>().ToListAsync();
            var expectedRoles = Main.StaticMigrations.DictEntities.RoleDeclaration.GetActual();
            actualRoles.Should().BeEquivalentTo(expectedRoles);
        }

        [Test]
        public async Task EnsureCreated_MainHistoricalWithoutAttribute()
        {
            await EnsureCreated(_dbContextMainHistoricalWithoutAttribute);

            var actual = await _dbContextMainHistoricalWithoutAttribute.Set<MainWithoutAttribute.Currency>().ToListAsync();
            var expected = MainWithoutAttribute.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            var actualRoles = await _dbContextMainHistoricalWithoutAttribute.Set<MainWithoutAttribute.Role>().ToListAsync();
            var expectedRoles = MainWithoutAttribute.StaticMigrations.DictEntities.RoleDeclaration.GetActual();
            actualRoles.Should().BeEquivalentTo(expectedRoles);
        }

        [Test]
        public async Task Migrate_Main_Should_Run()
        {
            await RunMigrations(_dbContextMain, true);
        }

        [Test]
        public async Task Migrate_MainHistoricalWithoutAttribute_Should_Run()
        {
            await RunMigrations(_dbContextMainHistoricalWithoutAttribute, true);
        }

        [Test]
        public async Task Registering_Historic_Migrations_Both_Ways_Should_Raise_Error()
        {
            Func<Task> act = async () => await RunMigrations(_dbContextMainTwoRegistrations, true);
            await act.Should().ThrowAsync<NotSupportedException>().Where(e => e.Message.StartsWith("Use one of options"));
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
