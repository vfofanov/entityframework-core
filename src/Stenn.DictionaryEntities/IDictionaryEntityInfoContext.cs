namespace Stenn.DictionaryEntities
{
    public interface IDictionaryEntityInfoContext
    {
        DictionaryEntityInfo<T> GetInfo<T>();
    }
}