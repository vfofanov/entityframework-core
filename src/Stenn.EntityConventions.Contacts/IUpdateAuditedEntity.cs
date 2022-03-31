using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Modified
    /// </summary>
    public interface IUpdateAuditedEntity
    {
        DateTime ModifiedAt => throw ExceptionHelper.ThrowRegistrationOnly();
    }
}