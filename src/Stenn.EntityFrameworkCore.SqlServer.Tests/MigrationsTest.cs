using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.DbContext.Initial;
using Stenn.EntityFrameworkCore.SqlServer.StaticMigrations;
using Stenn.EntityFrameworkCore.StaticMigrations;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class MigrationsTest
    {
        protected InitialDbContext InitialDbContext;
        protected MainDbContext MainDbContext;
        protected IServiceProvider ServiceProvider;

        protected static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            var dbName = "stenn_efcore_tests";
            var connectionString = GetConnectionString(dbName);

            void DbContextConfigure(DbContextOptionsBuilder builder)
            {
                builder.ReplaceService<IMigrator, MigratorWithStaticMigrations>();
                builder.UseSqlServer(connectionString);
                //builder.UseInMemoryDatabase(dbName);
            }

            services.AddScoped<IStaticMigrationHistoryRepositoryFactory, StaticMigrationHistoryRepositoryFactorySqlServer>();
            services.AddScoped<IStaticMigrationServiceFactory, StaticMigrationServiceFactory>();
            
            services.AddDbContext<InitialDbContext>(DbContextConfigure);
            services.AddDbContext<MainDbContext>(DbContextConfigure);

            ServiceProvider = services.BuildServiceProvider();
            InitialDbContext = ServiceProvider.GetRequiredService<InitialDbContext>();
            MainDbContext = ServiceProvider.GetRequiredService<MainDbContext>();
        }

        [Test]
        public async Task InitialMigration()
        {
            await RunMigrations(InitialDbContext);
            Assert.Pass();
        }
        [Test]
        public async Task MainMigration()
        {
            await RunMigrations(MainDbContext);
            Assert.Pass();
        }

        [Test]
        public async Task InitialAndMainMigration()
        {
            await RunMigrations(InitialDbContext, false);
            await RunMigrations(MainDbContext);
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