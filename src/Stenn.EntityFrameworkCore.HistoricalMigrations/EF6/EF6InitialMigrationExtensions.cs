using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations.EF6
{
    public static class EF6InitialMigrationExtensions
    {
        public static bool HasEF6InitialMigrationAttribute(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<EF6InitialMigrationAttribute>() is not null;
        }

        public static EF6InitialMigrationAttribute GetEF6InitialMigrationAttribute(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<EF6InitialMigrationAttribute>() ??
                   throw new InvalidOperationException();
        }

        public static bool IsEF6HistoryRepositoryExists(this ICurrentDbContext currentDbContext)
        {
            return currentDbContext.GetEF6HistoryRepository().Exists();
        }

        internal static IHistoryRepository GetEF6HistoryRepository(this ICurrentDbContext currentDbContext)
        {
            var repository = currentDbContext.Context.GetService<IHistoryRepository>();
            var dependencies = currentDbContext.Context.GetService<HistoryRepositoryDependencies>();

            //NOTE: Use hack with replace EF Core history table name to EF6 and only check exists
            var relationalOptions = RelationalOptionsExtension.Extract(dependencies.Options);
            var ef6RelationalOptions = relationalOptions.WithMigrationsHistoryTableName("__MigrationHistory");

            //NOTE: For replace relational options we must set exact type to WithExtension generic method
            var method = typeof(DbContextOptions).GetMethod(nameof(DbContextOptions.WithExtension))!;
            var generic = method.MakeGenericMethod(ef6RelationalOptions.GetType());
            var ef6DbContextOptions = (IDbContextOptions)generic.Invoke(dependencies.Options, new object?[] { ef6RelationalOptions })!;
            //var ef6DbContextOptions = ((DbContextOptions)dependencies.Options).WithExtension(ef6RelationalOptions);

#if NET5_0
            var ef6Dependincies = dependencies.With(ef6DbContextOptions);
#else
            var ef6Dependincies = new HistoryRepositoryDependencies(dependencies.DatabaseCreator, dependencies.RawSqlCommandBuilder, dependencies.Connection,
                ef6DbContextOptions, dependencies.ModelDiffer, dependencies.MigrationsSqlGenerator, dependencies.SqlGenerationHelper,
                dependencies.ConventionSetBuilder, dependencies.ModelDependencies, dependencies.TypeMappingSource, dependencies.CurrentContext,
                dependencies.ModelRuntimeInitializer,
#if NET6_0
#pragma warning disable CS0618
                dependencies.ModelLogger, 
#pragma warning restore CS0618
#endif
                dependencies.CommandLogger);
#endif

            var ef6HistoryRepository = (IHistoryRepository?)Activator.CreateInstance(repository.GetType(), ef6Dependincies);

            if (ef6HistoryRepository == null)
            {
                throw new EF6MigrateException($"Can't create instance of {repository.GetType()}. Can't check exist EF6 history table or not");
            }
            return ef6HistoryRepository;
        }
    }
}