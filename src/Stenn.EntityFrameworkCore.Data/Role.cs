using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.Data
{
    public class Role : IDictionaryEntity<Role>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <inheritdoc />
        bool IDictionaryEntity<Role>.EqualsByKey(Role other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        bool IDictionaryEntity<Role>.EqualsByProperties(Role other)
        {
            return Name == other.Name;
        }

        /// <inheritdoc />
        void IDictionaryEntity<Role>.CopyPropertiesFrom(Role source)
        {
            Name = source.Name;
        }
    }
}