using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityFrameworkCore.DbContext.Initial;

namespace Stenn.EntityFrameworkCore.SqlServer.Tests
{
    public class BaseTest
    {
        protected static string GetConnectionString(string dbName) => $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        protected InitialDbContext InitialDbContext;
        protected MainDbContext MainDbContext;
        protected IServiceProvider ServiceProvider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString("stenn_efcore_tests");
            services.AddDbContext<InitialDbContext>(builder =>
            {
                //builder.ReplaceService<>()
                builder.UseSqlServer(connectionString);
            });
            services.AddDbContext<MainDbContext>(builder =>
            {
                //builder.ReplaceService<>()
                builder.UseSqlServer(connectionString);
            });

            ServiceProvider = services.BuildServiceProvider();
            InitialDbContext = ServiceProvider.GetRequiredService<InitialDbContext>();
            MainDbContext = ServiceProvider.GetRequiredService<MainDbContext>();
        }

        [Test]
        public async Task Test()
        {
            await InitialDbContext.Database.EnsureCreatedAsync();

            try
            {
                await InitialDbContext.Database.MigrateAsync();

            }
            finally
            {
                await InitialDbContext.Database.EnsureDeletedAsync();
            }
            Assert.Pass();
        }
    }
}