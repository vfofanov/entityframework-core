using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Stenn.EntityFrameworkCore.EntityConventions;

namespace Stenn.EntityFrameworkCore.StaticMigrations.Conventions
{
    public class RelationalModelCustomizerWithConventions : RelationalModelCustomizer
    {
        private readonly IEntityConventionsProviderService _entityConventionsProviderService;

        /// <inheritdoc />
        public RelationalModelCustomizerWithConventions(ModelCustomizerDependencies dependencies,
            IEntityConventionsProviderService entityConventionsProviderService)
            : base(dependencies)
        {
            _entityConventionsProviderService = entityConventionsProviderService;
        }

        /// <inheritdoc />
        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            base.Customize(modelBuilder, context);

            var entityConventionsService = context.GetInfrastructure().GetRequiredService<IEntityConventionsService>();
            
            //NOTE: Convensions applying to root classes only
            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => e.BaseType is null))
            {
#pragma warning disable EF1001
                var entityBuilder = new EntityTypeBuilder(entityType);
#pragma warning restore EF1001
                entityConventionsService.Configure(entityBuilder);
                _entityConventionsProviderService.Configure(entityBuilder);
            }
        }
    }
}