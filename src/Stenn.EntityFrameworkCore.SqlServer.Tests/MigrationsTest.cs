using System;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;
using Stenn.EntityFrameworkCore.Tests;

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
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

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
        public async Task Migrate_Initial()
        {
            await RunMigrations(_dbContextInitial);

            await CheckSelect(_dbContextInitial, "SELECT * FROM dbo.ContactView", 
               async reader =>
               {
                 var count = 0;
                 while (await reader.ReadAsync())
                 {
                     count++;
                     reader.GetInt32(0).Should().Be(1);
                 }
                 count.Should().Be(1);
               });


            var actual = await _dbContextInitial.Set<CurrencyV1>().ToListAsync();
            var expected = Data.Initial.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Migrate_Main()
        {
            await Migrate_Main(true);
        }

        [Test]
        public async Task Migrate_MainWithoutDeletion()
        {
            await Migrate_Main(false);
        }

        [Test]
        public async Task Migrate_InitialThenMain()
        {
            await Migrate_Initial();
            await Migrate_Main(false);
        }

        private async Task Migrate_Main(bool deleteDb)
        {
            await RunMigrations(_dbContextMain, deleteDb);
            
            await CheckSelect(_dbContextInitial, "SELECT * FROM dbo.ContactView", 
                async reader =>
                {
                    var count = 0;
                    while (await reader.ReadAsync())
                    {
                        count++;
                        reader.GetInt32(0).Should().Be(2);
                    }
                    count.Should().Be(1);
                });
            
            var actual = await _dbContextMain.Set<Currency>().ToListAsync();
            var expected = Data.Main.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            var actualRoles = await _dbContextMain.Set<Role>().ToListAsync();
            var expectedRoles = Data.Main.StaticMigrations.DictEntities.RoleDeclaration.GetActual();
            actualRoles.Should().BeEquivalentTo(expectedRoles);
        }

        [Test]
        public void ExtractEnums_Main()
        {
            var enumTables = _dbContextMain.Model.ExtractEnumTables().ToList();
            
            enumTables.Should().HaveCount(3);

            var table = enumTables.First().Table;
            
            table.EnumType.Should().Be<ContactType>();
            table.ValueType.Should().Be<byte>();
            table.Rows.Should().HaveCount(2);

            table.Rows[0].RowShouldBe(1, "Person", "Person contact");
            table.Rows[1].RowShouldBe(2, "Organization", "Organization contact");
        }

        private static async Task CheckSelect(InitialDbContext context, string selectCommand, Func<DbDataReader, Task> check)
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = selectCommand;
            await context.Database.OpenConnectionAsync();
            await using var reader = await command.ExecuteReaderAsync();
            await check(reader);
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