#nullable enable
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityDefinition.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Model;
using Stenn.EntityDefinition.Model.Definitions;

namespace Stenn.EntityDefinition.InMemory.Tests
{
    [TestFixture]
    public class EntityDefinitionsTests
    {
        private const string DBName = "stenn_efcore_tests_in_memory";
        private DefinitionDbContext _dbContext = default!;
        private IServiceProvider _serviceProvider = default!;

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

            services.AddDbContext<TDbContext>(builder => { builder.UseInMemoryDatabase(DBName); });

            return services.BuildServiceProvider();
        }

        [Test]
        public void TestEntities()
        {
            var csv = _dbContext.Model.GenerateCsv(options =>
            {
                options.AddCommonConvert<bool>(CommonDefinitions.Converts.BoolToX);

                options.AddEntityColumn(CustomDefinitions.Domain.ToEntity());
                options.AddEntityColumn(EFCommonDefinitions.Entities.Name);
                options.AddEntityColumn(EFCommonDefinitions.Entities.Remark);
                options.AddEntityColumn(EFCommonDefinitions.Entities.IsObsolete);
                options.AddEntityColumn(EFCommonDefinitions.Entities.GetXmlDescription());
            });
        }

        [Test]
        public void TestProperties()
        {
            var csv = _dbContext.Model.GenerateCsv(options =>
            {
                options.AddCommonConvert<bool>(CommonDefinitions.Converts.BoolToX);

                options.AddEntityColumn(CustomDefinitions.Domain.ToEntity(), "Entity:Domain");
                options.AddEntityColumn(EFCommonDefinitions.Entities.Name, "Entity:Name");

                options.AddPropertyColumn(CustomDefinitions.Domain.ToProperty());
                options.AddPropertyColumn(EFCommonDefinitions.Properties.Name);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.Remark);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.IsObsolete);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.IsShadow);
                options.AddPropertyColumn(EFCommonDefinitions.Properties.GetXmlDescription());
            });
        }
    }
}