#nullable enable

namespace Stenn.DictionaryEntities.Contracts
{
    public interface IDictionaryEntity
    {
    }

    public interface IDictionaryEntity<in T> : IDictionaryEntity
        where T : IDictionaryEntity
    {
        bool EqualsByKey(T other);
        bool EqualsByProperties(T other);
        void CopyPropertiesFrom(T source);
    }
}