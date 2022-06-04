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
using Stenn.EntityFrameworkCore.HistoricalMigrations.EF6;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations
{
    public class HistoricalMigrationsAssembly : MigrationsAssembly
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly HistoryRepositoryDependencies _historyRepositoryDependencies;
        private readonly ICurrentDbContext _currentContext;
        private readonly IDbContextOptions _dbContextOptions;
        private readonly IMigrationsIdGenerator _idGenerator;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        private readonly HistoricalMigrationsOptions _options;

        /// <inheritdoc />
        public HistoricalMigrationsAssembly(
            ICurrentDbContext currentContext,
            IDbContextOptions dbContextOptions,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            IHistoryRepository historyRepository,
            HistoryRepositoryDependencies historyRepositoryDependencies)
            : base(currentContext, dbContextOptions, idGenerator, logger)
        {
            _historyRepository = historyRepository;
            _historyRepositoryDependencies = historyRepositoryDependencies;
            _currentContext = currentContext;
            _dbContextOptions = dbContextOptions;
            _idGenerator = idGenerator;
            _logger = logger;

            _options = dbContextOptions.FindExtension<HistoricalMigrationsOptionsExtension>().Options;

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
            if (migrations.SingleOrDefault(m => m.Value.HasEF6InitialMigrationAttribute()) is { Value: { } } ef6HistoricalMigration)
            {
                var initialMigrationId = ef6HistoricalMigration.Key;
                if (appliedMigrationEntrySet.Count == 0 &&
                    !_historyRepository.EF6HistoryRepositoryExists(_historyRepositoryDependencies))
                {
                    //NOTE: Retuns initial migration first
                    yield return ef6HistoricalMigration;
                }
                else
                {
                    var ef6Attr = ef6HistoricalMigration.Value.GetEF6InitialMigrationAttribute();
                    var manager = ef6Attr.GetManager();

                    //NOTE: Replace original initial migration with EF6InitialReplaceMigration
                    var initialReplaceMigration = CreateInitialMigrationReplaceType<EF6InitialReplaceMigration>(initialMigrationId, manager.MigrationIds);
                    yield return new KeyValuePair<string, TypeInfo>(initialMigrationId, initialReplaceMigration);    
                }
                appliedMigrationEntrySet.Add(initialMigrationId);
            }
            else if (migrations.SingleOrDefault(m => m.Value.HasHistoricalMigrationAttribute()) is { Value: { } } historicalMigration)
            {
                var historicalMigrationAttribute = historicalMigration.Value.GetHistoricalMigrationAttribute();
                var historicalMigrations = GetItems(historicalMigrationAttribute.DBContextAssemblyAnchorType);

                if (historicalMigrationAttribute.Initial)
                {
                    var initialMigrationId = historicalMigration.Key;
                    if (appliedMigrationEntrySet.Count == 0 && !_options.MigrateFromFullHistory)
                    {
                        //NOTE: Retuns initial migration first 
                        yield return historicalMigration;
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                    else if (!appliedMigrationEntrySet.Contains(initialMigrationId))
                    {
                        //NOTE: Returns all historical migrations and after remove history rows about them
                        var allHistoricalMigrationIds = new List<string>(historicalMigrations.Count);
                        foreach (var migration in PopulateMigrations(historicalMigrations, appliedMigrationEntrySet, allHistoricalMigrationIds))
                        {
                            yield return migration;
                        }

                        //NOTE: Replace original initial migration with InitialReplaceMigration if full history turned off
                        if (!_options.MigrateFromFullHistory)
                        {
                            var initialReplaceMigration =
                                CreateInitialMigrationReplaceType<InitialReplaceMigration>(initialMigrationId, allHistoricalMigrationIds.ToArray());
                            yield return new KeyValuePair<string, TypeInfo>(initialMigrationId, initialReplaceMigration);
                        }
                        appliedMigrationEntrySet.Add(initialMigrationId);
                    }
                }
                else
                {
                    foreach (var migration in PopulateMigrations(historicalMigrations, appliedMigrationEntrySet, allMigrationIds))
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
            if (!migrationClass.IsAssignableTo(typeof(ReplaceMigrationBase)))
            {
                return base.CreateMigration(migrationClass, activeProvider);
            }

            var removeMigrationRowIds = migrationClass.GetInitialMigration().RemoveMigrationRowIds;
            
            var migration = (Migration)Activator.CreateInstance(migrationClass.AsType(), 
                _historyRepository, removeMigrationRowIds)!;
            migration.ActiveProvider = activeProvider;

            return migration;
        }

        private IReadOnlyDictionary<string, TypeInfo> GetItems(Type dbContextType)
        {
            var migrationsAssembly = new ItemMigrationsAssembly(dbContextType, _currentContext, _dbContextOptions, _idGenerator, _logger);
            return migrationsAssembly.Migrations;
        }

        private static TypeInfo CreateInitialMigrationReplaceType<TReplaceMigration>(string migrationId, string[] migrationIds)
            where TReplaceMigration : ReplaceMigrationBase
        {
            var assemblyName = $"Assembly{migrationId}";
            var aName = new AssemblyName(assemblyName);
            var ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);

            // The module name is usually the same as the assembly name.
            var mb = ab.DefineDynamicModule(assemblyName);

            var parent = typeof(TReplaceMigration);
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

            Type[] parameterTypes =
            {
                typeof(IHistoryRepository),
                typeof(IReadOnlyCollection<string>)
            };

            var ctor1 = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            var ctor1IL = ctor1.GetILGenerator();
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Ldarg_1);
            ctor1IL.Emit(OpCodes.Ldarg_2);
            ctor1IL.Emit(OpCodes.Call, parent.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First());
            ctor1IL.Emit(OpCodes.Ret);

            // Finish the type.
            return tb.CreateType()!.GetTypeInfo();
        }
    }
}