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
            var csv = _dbContext.Model.GenerateCsv(options =>
            {
                options.AddCommonConvert<bool>(CommonDefinitions.Converts.BoolToX);
                
                options.AddEntityColumn(CustomDefinitions.Domain.ToEntity());
                options.AddEntityColumn(EFCommonDefinitions.Entities.Name);
                options.AddEntityColumn(EFRelationalDefinitions.Entities.DbName);
                options.AddEntityColumn(EFRelationalDefinitions.Entities.IsTable);
                options.AddEntityColumn(EFCommonDefinitions.Entities.Remark);
                options.AddEntityColumn(EFCommonDefinitions.Entities.IsObsolete);
                options.AddEntityColumn(EFCommonDefinitions.Entities.GetXmlDescription());

                options.AddPropertyColumn(CustomDefinitions.Domain.ToProperty());
                options.AddPropertyColumn(EFCommonDefinitions.Properties.Name, "Property Name");
                options.AddPropertyColumn(EFRelationalDefinitions.Properties.ColumnName);
                options.AddPropertyColumn(EFRelationalDefinitions.Properties.ColumnType);
                options.AddPropertyColumn(EFRelationalDefinitions.Properties.IsColumnNullable);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.Remark);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.IsObsolete, convertToString: x => x ? "X" : null);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.IsShadow);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.GetXmlDescription());
            });
        }
    }
}