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
using Microsoft.EntityFrameworkCore.Migrations.Internal;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public class SplittedMigrationsAssembly: MigrationsAssembly
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly ICurrentDbContext _currentContext;
        private readonly IDbContextOptions _options;
        private readonly IMigrationsIdGenerator _idGenerator;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;

        /// <inheritdoc />
        public SplittedMigrationsAssembly(
            ICurrentDbContext currentContext, 
            IDbContextOptions options, 
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger, 
            IHistoryRepository historyRepository) 
            : base(currentContext, options, idGenerator, logger)
        {
            _historyRepository = historyRepository;
            _currentContext = currentContext;
            _options = options;
            _idGenerator = idGenerator;
            _logger = logger;
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, TypeInfo> Migrations =>
            new Dictionary<string, TypeInfo>(PopulateMigrations(_historyRepository.GetAppliedMigrations().Select(t => t.MigrationId)));

        public IEnumerable<KeyValuePair<string, TypeInfo>> PopulateMigrations(IEnumerable<string> appliedMigrationEntries)
        {
            var appliedMigrationEntrySet = new HashSet<string>(appliedMigrationEntries, StringComparer.OrdinalIgnoreCase);
            return PopulateMigrations(base.Migrations, appliedMigrationEntrySet, null);
        }

        private IEnumerable<KeyValuePair<string, TypeInfo>> PopulateMigrations(IReadOnlyDictionary<string, TypeInfo> migrations,
            HashSet<string> appliedMigrationEntrySet, List<string>? allMigrationIds)
        {
            allMigrationIds ??= new List<string>(migrations.Count);
            var splittedMigration = migrations.SingleOrDefault(m => m.Value.HasSplittedMigrations());
            if (splittedMigration is { Value: { } })
            {
                var splittedMigrationsAttr = splittedMigration.Value.GetSplittedMigrations();
                var splittedMigrations = GetItems(splittedMigrationsAttr.DbContextType);

                if (splittedMigrationsAttr.Initial)
                {
                    var initialMigrationId = splittedMigration.Key;
                    if (appliedMigrationEntrySet.Count == 0)
                    {
                        //NOTE: Retuns initial migration first 
                        yield return splittedMigration;
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                    else if (!appliedMigrationEntrySet.Contains(initialMigrationId))
                    {
                        //NOTE: Returns all splitted migrations and after remove history rows about them
                        var allSplittedMigrationIds = new List<string>(splittedMigrations.Count);
                        foreach (var migration in PopulateMigrations(splittedMigrations, appliedMigrationEntrySet, allSplittedMigrationIds))
                        {
                            yield return migration;
                        }
                        var initialReplaceMigration = CreateInitialMigrationReplaceType(initialMigrationId, allSplittedMigrationIds.ToArray());
                        yield return new KeyValuePair<string, TypeInfo>(initialMigrationId, initialReplaceMigration);
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                }
                else
                {
                    foreach (var migration in PopulateMigrations(splittedMigrations, appliedMigrationEntrySet, allMigrationIds))
                    {
                        yield return migration;
                    }
                }
            }

            foreach (var migration in migrations)
            {
                allMigrationIds.Add(migration.Key);
                if (!appliedMigrationEntrySet.Contains(migration.Key))
                {
                    yield return migration;
                }
            }
        }

        /// <inheritdoc />
        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            if (!migrationClass.IsAssignableTo(typeof(InitialMigrationReplaceBase)))
            {
                return base.CreateMigration(migrationClass, activeProvider);
            }
            
            var removeMigrationRowIds = migrationClass.GetInitialMigration().RemoveMigrationRowIds;
            var migration = (Migration)Activator.CreateInstance(migrationClass.AsType(), _historyRepository, removeMigrationRowIds)!;
            migration.ActiveProvider = activeProvider;

            return migration;
        }

        private IReadOnlyDictionary<string, TypeInfo> GetItems(Type dbContextType)
        {
            var migrationsAssembly = new ItemMigrationsAssembly(dbContextType, _currentContext, _options, _idGenerator, _logger);
            return migrationsAssembly.Migrations;
        }

        private static TypeInfo CreateInitialMigrationReplaceType(string migrationId, string[] migrationIds)
        {
            var assemblyName = $"Assembly{migrationId}";
            var aName = new AssemblyName(assemblyName);
            var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            // The module name is usually the same as the assembly name.
            var mb = ab.DefineDynamicModule(assemblyName);

            var parent = typeof(InitialMigrationReplaceBase);
            var tb = mb.DefineType("InitialMigration", TypeAttributes.Public, parent);

            #region MigrationAttribute
            {
                var con = typeof(MigrationAttribute).GetConstructor(new[] { typeof(string) })!;
                tb.SetCustomAttribute(new CustomAttributeBuilder(con, new object?[] { migrationId }));
            }
            #endregion
            #region InitialMigrationAttribute
            {
                var con = typeof(InitialMigrationAttribute).GetConstructor(new[] { typeof(string[]) })!;
                tb.SetCustomAttribute(new CustomAttributeBuilder(con, new object?[] { migrationIds }));
            }
            #endregion

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
            return tb.CreateType()!.GetTypeInfo();
        }
    }
}