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
using Stenn.EntityFrameworkCore.Data.Main.HistoricalInitial;
using Stenn.EntityFrameworkCore.Data.Main.StaticMigrations;
using Stenn.EntityFrameworkCore.EntityConventions.TriggerBased;
using Stenn.EntityFrameworkCore.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.HistoricalMigrations.Extensions.DependencyInjection;
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
        private HistoricalInitialMainDbContext _dbContextMainHistoricalInitial = null!;

        private IServiceProvider _serviceProviderInitial = null!;
        private IServiceProvider _serviceProviderMain = null!;
        private IServiceProvider _serviceProviderMainHistoricalInitial = null!;

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        [SetUp]
        public void Setup()
        {
            InitDbContext(InitialStaticMigrations.Init, false, 
                out _serviceProviderInitial, 
                out _dbContextInitial);

            InitDbContext(MainStaticMigrations.Init, true,
                out _serviceProviderMain,
                out _dbContextMain);
            
            InitDbContext(MainStaticMigrations.Init, true,
                out _serviceProviderMainHistoricalInitial,
                out _dbContextMainHistoricalInitial);
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
                    builder.UseStaticMigrationsSqlServer(options =>
                    {
                        options.InitMigrations = init;
                        options.ConventionsOptions.IncludeCommonConventions = includeCommonConventions;
                        options.ConventionsOptions.InitEntityConventions = b =>
                        {
                            if (includeCommonConventions)
                            {
                                b.AddTriggerBasedCommonConventions();
                            }
                        };
                    });
                    builder.UseHistoricalMigrations();
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
        public async Task EnsureCreated_MainHistoricalInitial()
        {
            await EnsureCreated(_dbContextMainHistoricalInitial);

            var actual = await _dbContextMainHistoricalInitial.Set<Currency>().ToListAsync();
            var expected = Data.Main.StaticMigrations.DictEntities.CurrencyDeclaration.GetActual();
            actual.Should().BeEquivalentTo(expected);

            var actualRoles = await _dbContextMainHistoricalInitial.Set<Role>().ToListAsync();
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
        public async Task Migrate_MainHistoricalInitial()
        {
            await Migrate_MainHistoricalInitial(true);
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

        [Test]
        public async Task Migrate_InitialThenMainThenMainHistoricalInitial()
        {
            await Migrate_Initial();
            await Migrate_Main(false);
            await Migrate_MainHistoricalInitial(false);
        }
        
        private async Task Migrate_Main(bool deleteDb)
        {
            await RunMigrations(_dbContextMain, deleteDb);

            await CheckMain(_dbContextMain);
        }

        private async Task Migrate_MainHistoricalInitial(bool deleteDb)
        {
            await RunMigrations(_dbContextMainHistoricalInitial, deleteDb);

            await CheckMain(_dbContextMainHistoricalInitial);
        }
        
        private async Task CheckMain(Microsoft.EntityFrameworkCore.DbContext context)
        {
            await CheckSelect(context, "SELECT * FROM dbo.ContactView",
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

            enumTables.Should().HaveCount(5);

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
            var roleForDelete = Role.Create(newRoleDeleteId.ToString(), "ForDelete");
            await _dbContextMain.Set<Role>().AddAsync(roleForDelete);
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