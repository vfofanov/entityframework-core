using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.EntityFrameworkCore.SplittedMigrations.Extensions.DependencyInjection
{
    internal sealed class SplittedMigrationsOptionsExtension : IDbContextOptionsExtension
    {
        private ExtensionInfo? _info;
        public SplittedMigrationsOptions Options { get; }

        public SplittedMigrationsOptionsExtension(Type[] assemblyDbContextAnchors)
        {
            Options = new SplittedMigrationsOptions(assemblyDbContextAnchors);
        }

        /// <inheritdoc />
        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(Options);
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

            public ExtensionInfo(SplittedMigrationsOptionsExtension extension)
                : base(extension)
            {
            }

            private new SplittedMigrationsOptionsExtension Extension => (SplittedMigrationsOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment != null)
                    {
                        return _logFragment;
                    }
                    var builder = new StringBuilder();

                    builder.Append(
                        $"Assemblies=[{string.Join(";", Extension.Options.Anchors.Select(t => t.FullName))}] ");

                    _logFragment = builder.ToString();

                    return _logFragment;
                }
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (debugInfo == null)
                {
                    throw new ArgumentNullException(nameof(debugInfo));
                }

                debugInfo["Aggregate Migrations: Assemblies"] = string.Join(";", Extension.Options.Anchors.Select(t => t.FullName));
            }

            public override long GetServiceProviderHashCode()
            {
                if (_serviceProviderHash != null)
                {
                    return _serviceProviderHash.Value;
                }
                var hashCode = Extension.Options.GetHashCode();
                _serviceProviderHash = hashCode;

                return _serviceProviderHash.Value;
            }
        }
    }
}