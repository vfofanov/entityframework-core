using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public class SplittedMigrationsAssembly : MigrationsAssembly
    {
        private IReadOnlyDictionary<string, TypeInfo>? _migrations;
        private readonly SplittedMigrationsOptions _splittedOptions;
        private readonly ICurrentDbContext _currentContext;
        private readonly IDbContextOptions _options;
        private readonly IMigrationsIdGenerator _idGenerator;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;

        /// <inheritdoc />
        public SplittedMigrationsAssembly(
            ICurrentDbContext currentContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
            : base(currentContext, options, idGenerator, logger)
        {
            var splittedMigrationExtension = options.FindExtension<SplittedMigrationsOptionsExtension>();
            if (splittedMigrationExtension == null)
            {
                throw new InvalidOperationException("Can't find SplittedMigrationsOptionsExtension extension");
            }
            _splittedOptions = splittedMigrationExtension.Options;

            _currentContext = currentContext;
            _options = options;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, TypeInfo> Migrations
        {
            get
            {
                IReadOnlyDictionary<string, TypeInfo> Create()
                {
                    var result = new SortedList<string, TypeInfo>();

                    foreach (var anchor in _splittedOptions.Anchors)
                    {
                        var item = new ItemMigrationsAssembly(anchor, _currentContext, _options, _idGenerator, _logger);
                        foreach (var (key, migration) in item.Migrations)
                        {
                            result.Add(key, migration);
                        }
                    }
                    foreach (var (key, migration) in base.Migrations)
                    {
                        result.Add(key, migration);
                    }
                    return result;
                }

                return _migrations ??= Create();
            }
        }

        private sealed class ItemMigrationsAssembly : MigrationsAssembly
        {
            /// <summary>
            /// Set a private Property Value on a given Object. Uses Reflection.
            /// </summary>
            /// <typeparam name="T">Type of the Property</typeparam>
            /// <param name="obj">Object from where the Property Value is returned</param>
            /// <param name="propName">Propertyname as string.</param>
            /// <param name="val">the value to set</param>
            /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
            public static void SetPrivateFieldValue<T>(object obj, string propName, T val)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                var t = obj.GetType();
                FieldInfo? fi = null;
                while (fi == null && t != null)
                {
                    fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    t = t.BaseType;
                }
                if (fi == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
                }
                fi.SetValue(obj, val);
            }

            /// <inheritdoc />
            public ItemMigrationsAssembly(
                Type dbContextAnchorType,
                ICurrentDbContext currentContext,
                IDbContextOptions options,
                IMigrationsIdGenerator idGenerator,
                IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
                : base(currentContext, options, idGenerator, logger)
            {
                Assembly = dbContextAnchorType.Assembly;
                SetPrivateFieldValue(this, "_contextType", dbContextAnchorType);
            }

            /// <inheritdoc />
            public override Assembly Assembly { get; }

            /// <inheritdoc />
            public override ModelSnapshot ModelSnapshot
                => throw new NotSupportedException("Can't use snapshot from aggregated migrations assembly");
        }
    }
}