using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.EntityDefinition.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using Stenn.EntityDefinition.Model.Definitions;
using Stenn.EntityDefinition.Tests.Model;

namespace Stenn.EntityDefinition.InMemory.Tests
{
    public class EntityDefinitionsTests
    {
        private const string DBName = "stenn_efcore_tests_in_memory";
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

            services.AddDbContext<TDbContext>(builder => { builder.UseInMemoryDatabase(DBName); });

            return services.BuildServiceProvider();
        }

        [Test]
        public void Test()
        {
            var reader = new EntityFrameworkCoreDefinitionReader(_dbContext.Model,
                new IEFEntityDefinitionInfo[]
                {
                    CustomDefinitions.Domain.ToEntity(),
                    EFCommonDefinitions.Entities.Name,
                    EFCommonDefinitions.Entities.Remark,
                    EFCommonDefinitions.Entities.IsObsolete
                },
                new IEFPropertyDefinitionInfo[]
                {
                    CustomDefinitions.Domain.ToProperty(),
                    EFCommonDefinitions.Properties.Name,
                    EFCommonDefinitions.Properties.Remark,
                    EFCommonDefinitions.Properties.IsObsolete,
                    EFCommonDefinitions.Properties.IsShadow
                });

            var map = reader.Read();
        }
    }
}