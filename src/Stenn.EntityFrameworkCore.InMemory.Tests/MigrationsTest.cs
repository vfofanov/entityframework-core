using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Data.Initial.Migrations.Static;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Data.Main.Migrations.Static;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.InMemory.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.Testing;

namespace Stenn.EntityFrameworkCore.InMemory.Tests
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

            var actual = await _dbContextInitial.Set<CurrencyV1>().ToListAsync();
            var expected = Data.Initial.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task EnsureCreated_Main()
        {
            await EnsureCreated(_dbContextMain);

            var actual = await _dbContextMain.Set<Currency>().ToListAsync();
            var expected = Data.Main.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);
            
            var actualRoles = await _dbContextMain.Set<Role>().ToListAsync();
            var expectedRoles = Data.Main.StaticMigrations.DictEntities.RoleDeclaration.GetActual();
            actualRoles.Should().BeEquivalentTo(expectedRoles);
        }

        [Test]
        public void CheckMapping()
        {
            _dbContextInitial.CheckEntities();
            _dbContextMain.CheckEntities();
        }

        private static async Task EnsureCreated(Microsoft.EntityFrameworkCore.DbContext dbContext, bool deleteDb = true)
        {
            var database = dbContext.Database;
            if (deleteDb)
            {
                await database.EnsureDeletedAsync();
            }
            await database.EnsureCreatedAsync();
        }
    }
}