using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Created
    /// </summary>
    public interface ICreateAuditedEntity
    {
        DateTime Created => throw ExceptionHelper.ThrowRegistrationOnly();
    }
}