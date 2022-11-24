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
    public class SqlServerEntityDefinitionsTests
    {
        private const string DBName = "stenn_definitions_efcore_tests";

        private static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=SSPI";
        }

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

            var connectionString = GetConnectionString(DBName);

            services.AddDbContext<TDbContext>(builder => { builder.UseSqlServer(connectionString); },
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


        [Test]
        public void TestFlowchart()
        {
            //var entitiesTable = _dbContext.Model.GenerateEntityDefinitionTable(InitGraphEntitiesReaderOptions);
            //var propertiesTable = _dbContext.Model.GenerateEntityDefinitionTable(InitGraphReaderOptions);
            //----
            var options = new EFFlowchartGraphBuilderOptions();

            options.InitReaderOptions(opt =>
            {
                opt.AddEntityColumn(CustomDefinitions.Domain);
            });

            options.GraphGroupings.Add(CustomDefinitions.Domain.ToFlowchartGraphGrouping(DefinitionColumnType.Entity, (domain, styleClass) =>
                {
                    var color = domain switch
                    {
                        Domain.Unknown => Color.LightGray,
                        Domain.Security => Color.PaleVioletRed,
                        Domain.Order => Color.Aquamarine,
                        _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
                    };

                    styleClass.SetModifier("fill", ColorTranslator.ToHtml(color))
                        .SetModifier("stroke-width", "2px")
                        .SetModifier("stroke-dasharray", "2 2");
                })
            );

            options.Property.DrawAsNode = false;
            options.DrawRelationAsNode = true;
            
            //options.SetPropertyFilter(propertyRow => propertyRow.GetValueOrDefault(CustomDefinitions.IsDomainDifferent.Info));

            var graphBuilder = new EFFlowchartGraphBuilder(options);
            var outputEditor = graphBuilder.Build(_dbContext).ToString(MermaidPrintConfig.Normal);
            var outputHtml = graphBuilder.Build(_dbContext).ToString(MermaidPrintConfig.ForHtml);
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

        private void InitGraphEntitiesReaderOptions(IEntityFrameworkCoreDefinitionOptions options)
        {
            //Skip base type properties in inherited entities
            options.SetPropertiesFilter((e, p) => p.DeclaringType == e);
            options.ReaderOptions = ExcludeScalarProperties | ExcludeIgnoredProperties;

            options.AddEntityColumn(CustomDefinitions.Domain.ToEntity(), "Entity:Domain");
            options.AddEntityColumn(EFCommonDefinitions.Entities.Name, "Entity:Name");
            options.AddEntityColumn(EFRelationalDefinitions.Entities.DbName, "Entity:DbName");
            options.AddEntityColumn(EFCommonDefinitions.Entities.IsAbstract, "Entity: IsAbsctract");

            options.AddEntityColumn(EFCommonDefinitions.Entities.EntityType);
            options.AddEntityColumn(EFCommonDefinitions.Entities.BaseEntityType);
        }

        private void InitGraphReaderOptions(IEntityFrameworkCoreDefinitionOptions options)
        {
            InitGraphEntitiesReaderOptions(options);

            options.AddPropertyColumn(CustomDefinitions.Domain.ToProperty());
            options.AddPropertyColumn(CustomDefinitions.IsDomainDifferent.ToProperty());

            options.AddPropertyColumn(EFCommonDefinitions.Properties.Id);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Name);

            options.AddPropertyColumn(EFCommonDefinitions.Properties.IsNavigation);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.IsNavigationCollection);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.IsOnDependent);

            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.TargetEntityType);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.TargetPropertyId);

            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.DeclaringEntityType);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.DeclaringProperty);
            options.AddPropertyColumn(EFCommonDefinitions.Properties.Navigation.ForeignKey);
        }
    }
}