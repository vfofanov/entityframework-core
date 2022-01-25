using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.DictionaryEntities;

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
            
            _migrationsBuilder.Build(services);
            
            services.TryAddTransient<IStaticMigrationsService, StaticMigrationsService>();
            services.TryAddTransient<IDictionaryEntityMigrator, DbContextDictionaryEntityMigrator>();
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