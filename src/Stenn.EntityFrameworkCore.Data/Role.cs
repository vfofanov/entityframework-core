using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.Data
{
    [DebuggerDisplay("{Name}, Id = {Id}")]
    [DictionaryEntityIgnoredProperties(nameof(Created))]
    public class Role : AuditedEntity
    {
        public Role()
        {
            SourceSystemId = Id.ToString("N");
        }

        public string Name { get; private set; }
        public string Description { get; private set; }

        [JsonIgnore]
        public string SourceSystemId { get; private set; }

        public static Role Create(string id, string name, string desc = null, string sourceSystemId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            id = id.ToUpper();
            var idGuid = Guid.Parse(id);
            return new Role { Id = idGuid, Name = name, Description = desc, SourceSystemId = sourceSystemId ?? id };
        }
    }
}