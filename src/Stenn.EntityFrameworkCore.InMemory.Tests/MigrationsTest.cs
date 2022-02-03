using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class MigrationsTest
    {
        private const string DBName = "stenn_efcore_tests_in_memory";
        private InitialDbContext _dbContextInitial;

        private MainDbContext _dbContextMain;
        private IServiceProvider _serviceProviderInitial;
        private IServiceProvider _serviceProviderMain;

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

            services.AddDbContext<TDbContext>(builder =>
            {
                builder.UseInMemoryDatabase(DBName);
                builder.UseStaticMigrationsInMemoryDatabase(init);
            });

            return services.BuildServiceProvider();
        }

        [Test]
        public async Task EnsureCreated_Initial()
        {
            await EnsureCreated(_dbContextInitial);

            var currencyList = await _dbContextInitial.Set<Currency>().ToListAsync();

            currencyList.Should().HaveCount(1);
            currencyList.Should().ContainSingle(currency => currency.IsoNumericCode == 1 && currency.Iso3LetterCode == "TST");
        }

        [Test]
        public async Task EnsureCreated_Main()
        {
            await EnsureCreated(_dbContextMain);
            Assert.Pass();
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