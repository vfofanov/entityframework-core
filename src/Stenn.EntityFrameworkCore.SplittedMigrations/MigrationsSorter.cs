#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public class MigrationsSorter
    {
        private readonly IMigrationsAssembly _migrationsAssembly;
        private readonly IHistoryRepository _historyRepository;
        private readonly ICurrentDbContext _currentContext;
        private readonly IDbContextOptions _options;
        private readonly IMigrationsIdGenerator _idGenerator;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        private readonly string _activeProvider;

        public MigrationsSorter(
            IMigrationsAssembly migrationsAssembly,
            IHistoryRepository historyRepository,
            ICurrentDbContext currentContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            IDatabaseProvider databaseProvider)
        {
            _migrationsAssembly = migrationsAssembly;
            _historyRepository = historyRepository;
            _currentContext = currentContext;
            _options = options;
            _idGenerator = idGenerator;
            _logger = logger;
            _activeProvider = databaseProvider.Name;
        }

        public IEnumerable<Migration> PopulateMigrations(IEnumerable<string> appliedMigrationEntries)
        {
            var appliedMigrationEntrySet = new HashSet<string>(appliedMigrationEntries, StringComparer.OrdinalIgnoreCase);
            return PopulateMigrations(_migrationsAssembly, appliedMigrationEntrySet, null);
        }

        private IEnumerable<Migration> PopulateMigrations(IMigrationsAssembly migrationsAssembly, HashSet<string> appliedMigrationEntrySet, List<string>? allMigrationIds)
        {
            allMigrationIds ??= new List<string>(migrationsAssembly.Migrations.Count);
            var splittedMigration = migrationsAssembly.Migrations.SingleOrDefault(m => m.Value.HasSplittedMigrations());
            if (splittedMigration is { Value: { } })
            {
                var splittedMigrationsAttr = splittedMigration.Value.GetSplittedMigrations();
                var splittedMigrationsAssembly = GetItems(splittedMigrationsAttr.DbContextType);

                if (splittedMigrationsAttr.Initial)
                {
                    var initialMigrationId = splittedMigration.Key;
                    if (appliedMigrationEntrySet.Count == 0)
                    {
                        //NOTE: Retuns initial migration first 
                        yield return migrationsAssembly.CreateMigration(splittedMigration.Value, _activeProvider);
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                    else if (!appliedMigrationEntrySet.Contains(initialMigrationId))
                    {
                        //NOTE: Returns all splitted migrations and after remove history rows about them
                        var allSplittedMigrationIds = new List<string>(splittedMigrationsAssembly.Migrations.Count);
                        foreach (var migration in PopulateMigrations(splittedMigrationsAssembly, appliedMigrationEntrySet, allSplittedMigrationIds))
                        {
                            yield return migration;
                        }
                        var initialReplaceMigration = CreateReplaceInitialMigration(initialMigrationId, allSplittedMigrationIds);
                        yield return initialReplaceMigration;
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                }
                else
                {
                    foreach (var migration in PopulateMigrations(splittedMigrationsAssembly, appliedMigrationEntrySet, allMigrationIds))
                    {
                        yield return migration;
                    }
                }
            }

            foreach (var (name, migrationType) in migrationsAssembly.Migrations)
            {
                allMigrationIds.Add(name);
                if (!appliedMigrationEntrySet.Contains(name))
                {
                    yield return migrationsAssembly.CreateMigration(migrationType, _activeProvider);
                }
            }
        }

        private ItemMigrationsAssembly GetItems(Type dbContextType)
        {
            return new ItemMigrationsAssembly(dbContextType, _currentContext, _options, _idGenerator, _logger);
        }

        private Migration CreateReplaceInitialMigration(string migrationId, List<string> migrationIds)
        {
            var type = CreateReplaceInitialMigrationType(migrationId);

            var migration = (Migration)Activator.CreateInstance(type, _historyRepository, migrationIds)!;
                
            return migration;
        }
        private static Type CreateReplaceInitialMigrationType(string migrationId)
        {
            var assemblyName = $"Assembly{migrationId}";
            var aName = new AssemblyName(assemblyName);
            var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            // The module name is usually the same as the assembly name.
            var mb = ab.DefineDynamicModule(assemblyName);

            var parent = typeof(InitialMigrationBase);
            var tb = mb.DefineType("InitialMigration", TypeAttributes.Public, parent);

            var con = typeof(MigrationAttribute).GetConstructor(new[] { typeof(string) })!;
            tb.SetCustomAttribute(new CustomAttributeBuilder(con, new object?[] { migrationId }));

            Type[] parameterTypes = { typeof(IHistoryRepository), typeof(List<string>) };
            var ctor1 = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            var ctor1IL = ctor1.GetILGenerator();
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Ldarg_1);
            ctor1IL.Emit(OpCodes.Ldarg_2);
            ctor1IL.Emit(OpCodes.Call, parent.GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).First());
            ctor1IL.Emit(OpCodes.Ret);

            // Finish the type.
            return tb.CreateType()!;
        }
    }
}