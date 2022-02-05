using System;

namespace Stenn.EntityFrameworkCore.Data
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }
    }
}