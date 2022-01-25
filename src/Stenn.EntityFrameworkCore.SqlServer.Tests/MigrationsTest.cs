using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;

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

            services.AddDbContext<TDbContext>(builder =>
            {
                builder.UseSqlServer(connectionString);
                builder.UseStaticMigrationsSqlServer(init);

                //builder.UseInMemoryDatabase(dbName);
                //builder.UseStaticMigrationsInMemoryDatabase(init);
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
            await RunMigrations(_dbContextInitial);
            await RunMigrations(_dbContextMain, false);
            Assert.Pass();
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
    }
}