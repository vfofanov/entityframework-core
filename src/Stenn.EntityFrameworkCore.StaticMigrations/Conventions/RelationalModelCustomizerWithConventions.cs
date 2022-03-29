using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stenn.EntityFrameworkCore.EntityConventions;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Conventions
{
    public class RelationalModelCustomizerWithConventions : RelationalModelCustomizer
    {
        private readonly IConventionsService _conventionsService;

        /// <inheritdoc />
        public RelationalModelCustomizerWithConventions(ModelCustomizerDependencies dependencies, 
            IConventionsService conventionsService)
            : base(dependencies)
        {
            _conventionsService = conventionsService;
        }

        /// <inheritdoc />
        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            base.Customize(modelBuilder, context);

            Console.WriteLine("--Start conventions--");
            
            //NOTE: Convensions applying to root classes only
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => e.BaseType is null))
            {
#pragma warning disable EF1001
                var entityBuilder = new EntityTypeBuilder(entityType);
#pragma warning restore EF1001
                _conventionsService.Configure(entityBuilder);
            }
        }
    }
}