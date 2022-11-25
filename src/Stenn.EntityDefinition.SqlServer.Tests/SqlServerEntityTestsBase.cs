#nullable enable
using System;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityDefinition.Contracts.Table;
using Stenn.EntityDefinition.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.EntityFrameworkCore.Flowchart;
using Stenn.EntityDefinition.EntityFrameworkCore.Relational;
using Stenn.EntityDefinition.Flowchart;
using Stenn.EntityDefinition.Model;
using Stenn.EntityDefinition.Model.Definitions;
using Stenn.Shared.Mermaid;
using static Stenn.EntityDefinition.EntityFrameworkCore.EntityFrameworkDefinitionReaderOptions;

namespace Stenn.EntityDefinition.SqlServer.Tests
{
    [TestFixture]
    public abstract class SqlServerEntityTestsBase
    {
        private const string DBName = "stenn_definitions_efcore_tests";

        protected static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

        protected DefinitionDbContext _dbContext = default!;

        protected IServiceProvider _serviceProvider = default!;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = GetServices<DefinitionDbContext>();
            _dbContext = _serviceProvider.GetRequiredService<DefinitionDbContext>();
        }

        private static IServiceProvider GetServices<TDbContext>()
            where TDbContext : DbContext
        {
            var services = new ServiceCollection();

            var connectionString = GetConnectionString(DBName);

            services.AddDbContext<TDbContext>(builder => { builder.UseSqlServer(connectionString); },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services.BuildServiceProvider();
        }
    }
}