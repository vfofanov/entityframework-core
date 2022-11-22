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
        public void TestCsvEntities()
        {
            var csv = _dbContext.Model.GenerateCsv(InitEntitiesReaderOptions);
        }
        [Test]
        public void TestCsvProperties()
        {
            var csv = _dbContext.Model.GenerateCsv(InitPropertiesReaderOptions);
        }
        
        [Test]
        public void TestDefTableEntities()
        {
            var table = _dbContext.Model.GenerateEntityDefinitionTable(InitEntitiesReaderOptions);
        }
        [Test]
        public void TestDefTableProperties()
        {
            var table = _dbContext.Model.GenerateEntityDefinitionTable(InitPropertiesReaderOptions);
        }
        
        private static void InitEntitiesReaderOptions(IEntityFrameworkCoreDefinitionOptions options)
        {
            options.AddCommonConvert<bool>(CommonDefinitions.Converts.BoolToX);

            options.AddEntityColumn(CustomDefinitions.Domain.ToEntity());
            options.AddEntityColumn(EFCommonDefinitions.Entities.Name);
            options.AddEntityColumn(EFCommonDefinitions.Entities.BaseEntityName);
            options.AddEntityColumn(EFRelationalDefinitions.Entities.DbName);
            options.AddEntityColumn(EFRelationalDefinitions.Entities.Type);
            options.AddEntityColumn(EFCommonDefinitions.Entities.Remark);
            options.AddEntityColumn(EFCommonDefinitions.Entities.IsObsolete);
            options.AddEntityColumn(EFCommonDefinitions.Entities.GetXmlDescription());
        }

        private void InitPropertiesReaderOptions(IEntityFrameworkCoreDefinitionOptions options)
        {
            options.AddCommonConvert<bool>(CommonDefinitions.Converts.BoolToX);

            options.AddEntityColumn(CustomDefinitions.Domain.ToEntity(), "Entity:Domain");
            options.AddPropertyColumn(CustomDefinitions.Domain.ToProperty());
            options.AddPropertyColumn(CustomDefinitions.IsDomainDifferent.ToProperty());

            options.AddEntityColumn(EFCommonDefinitions.Entities.Name, "Entity:Name");
            options.AddEntityColumn(EFRelationalDefinitions.Entities.DbName, "Entity:DbName");
            options.AddEntityColumn(EFRelationalDefinitions.Entities.Type, "Entity:Type");

            options.AddPropertyColumn(EFCommonDefinitions.Properties.Name);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.ClrType);
            options.AddPropertyColumn(EFRelationalDefinitions.Properties.ColumnName);
            options.AddPropertyColumn(EFRelationalDefinitions.Properties.DbColumnType);
            options.AddPropertyColumn(EFRelationalDefinitions.Properties.IsColumnNullable);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Remark);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.IsNavigation);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.IsObsolete);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.IsShadow);
            options.AddPropertyColumn(EFRelationalDefinitions.Properties.IsComputed);
            options.AddPropertyColumn(EFRelationalDefinitions.Properties.ComputedSql);

            options.AddPropertyColumn(EFCommonDefinitions.Properties.GetXmlDescription());
        }
    }
}