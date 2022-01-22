using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.DbContext.Initial;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class MigrationsTest
    {
        private const string DBName = "stenn_efcore_tests";
        private InitialDbContext _dbContextInitial;

        private MainDbContext _dbContextMain;
        private IServiceProvider _serviceProviderInitial;
        private IServiceProvider _serviceProviderMain;

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        [SetUp]
        public void Setup()
        {
            _serviceProviderInitial = GetServices<InitialDbContext>(InitialStaticMigrations.Init);
            _dbContextInitial = _serviceProviderInitial.GetRequiredService<InitialDbContext>();

            _serviceProviderMain = GetServices<MainDbContext>(MainStaticMigrations.Init);
            _dbContextMain = _serviceProviderMain.GetRequiredService<MainDbContext>();
        }

        private static IServiceProvider GetServices<TDbContext>(Action<StaticMigrationBuilder> init)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString(DBName);

            services.AddStaticMigrations<SqlServerMigrations>(init);
            services.AddTransient<IStaticMigrationHistoryRepositoryFactory, StaticMigrationHistoryRepositoryFactorySqlServer>();
            services.AddTransient<IStaticMigrationServiceFactory, StaticMigrationServiceFactory>();

            services.AddDbContext<TDbContext>((provider, builder) =>
            {
                builder.UseStaticMigrations(provider);
                builder.UseSqlServer(connectionString);
                //builder.UseInMemoryDatabase(dbName);
            });

            return services.BuildServiceProvider();
        }

        

        [Test]
        public async Task InitialMigration()
        {
            await RunMigrations(_dbContextInitial);
            Assert.Pass();
        }

        [Test]
        public async Task MainMigration()
        {
            await RunMigrations(_dbContextMain);
            Assert.Pass();
        }

        [Test]
        public async Task InitialAndMainMigration()
        {
            await RunMigrations(_dbContextInitial, false);
            await RunMigrations(_dbContextMain);
            Assert.Pass();
        }

        private static async Task RunMigrations(Microsoft.EntityFrameworkCore.DbContext dbContext, bool deleteDb = true)
        {
            var database = dbContext.Database;
            try
            {
                await database.MigrateAsync();
            }
            finally
            {
                if (deleteDb)
                {
                    await database.EnsureDeletedAsync();
                }
            }
        }
    }
}