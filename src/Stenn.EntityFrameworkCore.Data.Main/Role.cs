using System;
using System.Diagnostics;
using Stenn.Conventions.Contacts;

namespace Stenn.EntityFrameworkCore.Data.Main
{
    [DebuggerDisplay("{Name}, Id = {Id}")]
    public class Role : Entity, 
        ICreateAuditedEntity, 
        IUpdateAuditedEntity,  
        IEntityWithSourceSystemIdGuid,
        ISoftDeleteEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public static Role Create(string id, string name, string desc = null)
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
            return new Role { Id = idGuid, Name = name, Description = desc };
        }
    }
}