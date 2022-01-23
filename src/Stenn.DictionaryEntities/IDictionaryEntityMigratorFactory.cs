namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityMigratorFactory
    {
        IDictionaryEntityMigrator Create(IDictionaryEntityContext context);
    }
}