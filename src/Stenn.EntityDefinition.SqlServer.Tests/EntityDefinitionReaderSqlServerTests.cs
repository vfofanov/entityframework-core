using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityDefinition.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.EntityFrameworkCore.Relational;
using Stenn.EntityDefinition.Model;
using Stenn.EntityDefinition.Model.Definitions;

namespace Stenn.EntityDefinition.SqlServer.Tests
{
    public class EntityDefinitionsTests
    {
        private const string DBName = "stenn_definitions_efcore_tests";
        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }
        
        private DefinitionDbContext _dbContext;

        private IServiceProvider _serviceProvider;

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

            services.AddDbContext<TDbContext>(builder =>
                {
                    builder.UseSqlServer(connectionString);
                },
                ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services.BuildServiceProvider();
        }

        [Test]
        public void Test()
        {
            var reader = new EntityFrameworkCoreDefinitionReader(_dbContext.Model,
                new IEFEntityDefinition[]
                {
                    CustomDefinitions.Domain.ToEntity(),
                    EFCommonDefinitions.Entities.Name,
                    EFRelationalDefinitions.Entities.DbName,
                    EFRelationalDefinitions.Entities.IsTable,
                    EFCommonDefinitions.Entities.Remark,
                    EFCommonDefinitions.Entities.IsObsolete,
                    EFCommonDefinitions.Entities.GetXmlDescription()
                },
                new IEFPropertyDefinitionInfo[]
                {
                    CustomDefinitions.Domain.ToProperty(),
                    EFCommonDefinitions.Properties.Name,
                    EFRelationalDefinitions.Properties.ColumnName,
                    EFRelationalDefinitions.Properties.ColumnType,
                    EFRelationalDefinitions.Properties.IsColumnNullable,
                    EFCommonDefinitions.Properties.Remark,
                    EFCommonDefinitions.Properties.IsObsolete,
                    EFCommonDefinitions.Properties.IsShadow,
                    EFCommonDefinitions.Properties.GetXmlDescription()
                });

            var map = reader.Read();
        }
    }
}