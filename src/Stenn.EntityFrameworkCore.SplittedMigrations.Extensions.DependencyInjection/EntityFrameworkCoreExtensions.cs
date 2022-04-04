using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stenn.EntityFrameworkCore.SplittedMigrations.Extensions.DependencyInjection
{
    /// <summary>
    ///     Dependency injection extensions for register Entity Framework core static migrations
    /// </summary>
    public static class EntityFrameworkCoreExtensions
    {
        /// <summary>
        /// Add splitted migrations
        /// </summary>
        /// <param name="optionsBuilder">Db context options builder</param>
        /// <param name="init">Initialize action</param>
        /// <returns></returns>
        public static DbContextOptionsBuilder AddSplittedMigrations(this DbContextOptionsBuilder optionsBuilder,
            Action<SplittedMigrationsBuilder> init)
        {
            var extension = optionsBuilder.Options.FindExtension<SplittedMigrationsOptionsExtension>();
            if (extension != null)
            {
                throw new InvalidOperationException("Splitted migrations are already registered");
            }
            optionsBuilder.ReplaceService<IMigrationsAssembly, SplittedMigrationsAssembly>();

            var builder = new SplittedMigrationsBuilder();
            init(builder);
            extension = new SplittedMigrationsOptionsExtension(builder.Anchors.ToArray());
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }

    public sealed class SplittedMigrationsBuilder
    {
        internal List<Type> Anchors { get; }

        internal SplittedMigrationsBuilder()
        {
            Anchors = new();
        }

        /// <summary>
        /// Add type of splitted migrations' db context
        /// </summary>
        /// <typeparam name="TDbConextAnchor"></typeparam>
        public SplittedMigrationsBuilder Add<TDbConextAnchor>()
            where TDbConextAnchor : DbContext
        {
            Anchors.Add(typeof(TDbConextAnchor));
            return this;
        }
    }
}