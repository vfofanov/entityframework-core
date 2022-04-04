#nullable enable
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.Data.Initial;
using Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations;
using Stenn.EntityFrameworkCore.Data.Main;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SplittedMigrations.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.SqlServer.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.StaticMigrations.Enums;
using Stenn.EntityFrameworkCore.Tests;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class MigrationsTest
    {
        private const string DBName = "stenn_efcore_tests";
        private InitialDbContext _dbContextInitial = null!;

        private MainDbContext _dbContextMain = null!;
        private IServiceProvider _serviceProviderInitial = null!;
        private IServiceProvider _serviceProviderMain = null!;

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        [SetUp]
        public void Setup()
        {
            _serviceProviderInitial = GetServices<InitialDbContext>(InitialStaticMigrations.Init, false);
            _dbContextInitial = _serviceProviderInitial.GetRequiredService<InitialDbContext>();

            _serviceProviderMain = GetServices<MainDbContext>(MainStaticMigrations.Init, true, builder =>
            {
                //NOTE: Add migrations splitted to separate assemblies
                builder.AddSplittedMigrations(
                    b => b.Add<MainDbContext_Step1>()
                        .Add<MainDbContext_Step2>());
            });
            _dbContextMain = _serviceProviderMain.GetRequiredService<MainDbContext>();
        }

        private static IServiceProvider GetServices<TDbContext>(Action<StaticMigrationBuilder> init, bool includeCommonConventions,
            Action<DbContextOptionsBuilder>? additionalInit=null)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString(DBName);

            services.AddDbContext<TDbContext>(builder =>
                {
                    builder.UseSqlServer(connectionString);
                    builder.UseStaticMigrationsSqlServer(options =>
                    {
                        options.InitMigrations = init;
                        options.ConventionsOptions.IncludeCommonConventions = includeCommonConventions;
                    });
                    
                    additionalInit?.Invoke(builder);
                },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

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

        [Test, Explicit]
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
            
            enumTables.Should().HaveCount(4);

            var table = enumTables.First().Table;
            
            table.EnumType.Should().Be<ContactType>();
            table.ValueType.Should().Be<byte>();
            table.Rows.Should().HaveCount(2);

            table.Rows[0].RowShouldBe(1, "Person", "Person contact");
            table.Rows[1].RowShouldBe(2, "Organization", "Organization contact");
        }

        [Test]
        public async Task SoftDeleteRoles()
        {
            await EnsureCreated_Main();

            var originCount = _dbContextMain.Set<Role>().Count();

            var newRoleDeleteId = Guid.NewGuid();
            await _dbContextMain.Set<Role>().AddAsync(Role.Create(newRoleDeleteId.ToString(), "ForDelete"));
            await _dbContextMain.SaveChangesAsync();
            
            var afterAddCount = _dbContextMain.Set<Role>().Count();
            afterAddCount.Should().Be(originCount + 1);

            await Task.Delay(1000);
            var modifiedRole = await _dbContextMain.Set<Role>().FirstAsync(x => x.Name == "Customer");
            modifiedRole.Description = "MODIFIED DESC";
            await _dbContextMain.SaveChangesAsync();
            
            var roleToDelete = await _dbContextMain.Set<Role>().SingleAsync(x => x.Id == newRoleDeleteId);
            _dbContextMain.Set<Role>().Remove(roleToDelete);
            await _dbContextMain.SaveChangesAsync();
            
            var afterRemoveCount = _dbContextMain.Set<Role>().Count();
            afterRemoveCount.Should().Be(originCount);
            //NOTE: Check that deleted row exists in db(Soft delete)
            await CheckSelect(_dbContextMain, "SELECT * FROM dbo.Role", 
                async reader =>
                {
                    var count = 0;
                    while (await reader.ReadAsync())
                    {
                        count++;
                    }
                    count.Should().Be(afterAddCount);
                });
        }

        [TestCase(1000)]
        public async Task CreateMultipleRequests(int countOfParallelRequests)
        {
            await Migrate_Main();
            Enumerable.Range(0, countOfParallelRequests).AsParallel().ForAll(i =>
            {
                using var scope = _serviceProviderMain.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
                var currencies = dbContext.Set<Currency>().ToList();
                Console.WriteLine($"Id: {i}, ContextId:{dbContext.ContextId.InstanceId}, Count:{currencies.Count}");
            });

        }

        private static async Task CheckSelect(Microsoft.EntityFrameworkCore.DbContext context, string selectCommand, Func<DbDataReader, Task> check)
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