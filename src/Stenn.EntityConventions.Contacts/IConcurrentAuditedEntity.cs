using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with concurrent row version property
    /// </summary>
    public interface IConcurrentAuditedEntity
    {
        byte[] RowVersion => throw ExceptionHelper.ThrowRegistrationOnly();
    }
}