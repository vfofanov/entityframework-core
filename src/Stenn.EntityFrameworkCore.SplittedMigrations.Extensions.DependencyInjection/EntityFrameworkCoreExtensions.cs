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
        /// Add splitted migrations as migrations for context
        /// </summary>
        /// <param name="optionsBuilder">Db context options builder</param>
        /// <typeparam name="TDbConextAnchor">DbContext splitted migrations anchor for assembly with splitted migrations</typeparam>
        /// <returns></returns>
        public static DbContextOptionsBuilder AddSplittedMigrations<TDbConextAnchor>(
            this DbContextOptionsBuilder optionsBuilder)
        where TDbConextAnchor:DbContext
        {
            optionsBuilder.ReplaceService<IMigrationsAssembly, SplittedMigrationsAssembly>();
            
            
            var extension = optionsBuilder.Options.FindExtension<SplittedMigrationsOptionsExtension>();

            var anchors = extension == null 
                ? new List<Type>() 
                : new List<Type>(extension.Options.Anchors);
            anchors.Add(typeof(TDbConextAnchor));
            
            extension = new SplittedMigrationsOptionsExtension(anchors.ToArray());
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}