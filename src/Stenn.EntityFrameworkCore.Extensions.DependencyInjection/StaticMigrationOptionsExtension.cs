using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.DictionaryEntities;
using Stenn.EntityFrameworkCore.StaticMigrations;
using Stenn.StaticMigrations;

namespace Stenn.EntityFrameworkCore.Extensions.DependencyInjection
{
    internal sealed class StaticMigrationOptionsExtension : IDbContextOptionsExtension
    {
        private ExtensionInfo? _info;
        private readonly IStaticMigrationsProviderConfigurator _configurator;
        private readonly StaticMigrationBuilder _migrationsBuilder;
        
        public StaticMigrationOptionsExtension(IStaticMigrationsProviderConfigurator configurator, StaticMigrationBuilder migrationsBuilder)
        {
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
            _migrationsBuilder = migrationsBuilder ?? throw new ArgumentNullException(nameof(migrationsBuilder));
        }

        /// <inheritdoc />
        public void ApplyServices(IServiceCollection services)
        {
            _configurator.RegisterServices(services);

            services.TryAddScoped<IStaticMigrationsService, StaticMigrationsService>();
            
            //_migrationsBuilder.Build(services);
            
#pragma warning disable EF1001
            services.AddScoped<IStaticMigrationCollection<IStaticSqlMigration, DbContext>>(
                provider => provider.GetRequiredService<IDbContextServices>().ContextOptions
                    .FindExtension<StaticMigrationOptionsExtension>()._migrationsBuilder.SQLMigrations);
            
            services.AddScoped<IStaticMigrationCollection<IDictionaryEntityMigration, DbContext>>(
                provider => provider.GetRequiredService<IDbContextServices>().ContextOptions
                    .FindExtension<StaticMigrationOptionsExtension>()._migrationsBuilder.DictEntityMigrations);
#pragma warning restore EF1001
            
            services.TryAddScoped<IStaticMigrationsService, StaticMigrationsService>();
            services.TryAddScoped<IDictionaryEntityMigrator, DbContextDictionaryEntityMigrator>();

            var relationalOverrided = OverrideService<IRelationalDatabaseCreator>(services, (provider, creator) =>
                new RelationalDatabaseCreatorWithStaticMigrations(creator,
                    provider, 
                    provider.GetRequiredService<RelationalDatabaseCreatorDependencies>(), 
                    provider.GetRequiredService<IRelationalConnection>(), 
                    provider.GetRequiredService<IMigrationCommandExecutor>(), 
                    provider.GetRequiredService<IMigrationsSqlGenerator>()));

            if (relationalOverrided)
            {
                return;
            }

            services.TryAddScoped<IStaticMigrationHistoryRepository, EmptyStaticMigrationHistoryRepository>();
            var overrided = OverrideService<IDatabaseCreator>(services, (provider, creator) =>
                new DatabaseCreatorWithStaticMigrations(creator, provider.GetRequiredService<IStaticMigrationsService>()));

            if (!overrided)
            {
                throw new NotSupportedException("Can't use static migrations with this database provider. Can;t find service 'IDatabaseCreator'");
            }
        }

        private static bool OverrideService<T>(IServiceCollection services, Func<IServiceProvider, T, T> factory)
            where T : notnull
        {
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (descriptor == null)
            {
                return false;
            }
            if (descriptor.ImplementationType != null)
            {
                services.Add(new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType, descriptor.Lifetime));
            }
            services.Replace(new ServiceDescriptor(typeof(T), provider =>
            {
                T baseService;
                if (descriptor.ImplementationInstance != null)
                {
                    baseService = (T)descriptor.ImplementationInstance;
                }
                else if (descriptor.ImplementationType != null)
                {
                    baseService = (T)provider.GetRequiredService(descriptor.ImplementationType);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    baseService = (T)descriptor.ImplementationFactory.Invoke(provider);
                }
                else
                {
                    throw new NotSupportedException();
                }

                return factory(provider, baseService);
            }, descriptor.Lifetime));

            return true;
        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
        }

        /// <inheritdoc />
        public DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);
        
        
        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private long? _serviceProviderHash;
            private string? _logFragment;

            public ExtensionInfo(StaticMigrationOptionsExtension extension)
                : base(extension)
            {
            }

            private new StaticMigrationOptionsExtension Extension => (StaticMigrationOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append($"Configurator={Extension._configurator.GetType().Name} ");
                        
                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (debugInfo == null)
                {
                    throw new ArgumentNullException(nameof(debugInfo));
                }

                debugInfo["Static Migrations: Configurator"] = Extension._configurator.GetType().Name;
            }

            public override long GetServiceProviderHashCode()
            {
                if (_serviceProviderHash != null)
                {
                    return _serviceProviderHash.Value;
                }
                var hashCode = Extension._configurator.GetType().GetHashCode();
                _serviceProviderHash = hashCode;

                return _serviceProviderHash.Value;
            }
        }
    }
}