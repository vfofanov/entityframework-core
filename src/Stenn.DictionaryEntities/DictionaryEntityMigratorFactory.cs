namespace Stenn.DictionaryEntities
{
    public sealed class DictionaryEntityMigratorFactory : IDictionaryEntityMigratorFactory
    {
        /// <inheritdoc />
        public IDictionaryEntityMigrator Create(IDictionaryEntityContext context)
        {
            return new DictionaryEntityMigrator(context);
        }
    }
}