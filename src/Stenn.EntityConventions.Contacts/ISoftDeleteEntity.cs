using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Modified
    /// </summary>
    public interface ISoftDeleteEntity
    {
        bool IsDeleted => throw ExceptionHelper.ThrowRegistrationOnly();
        DateTime? Deleted => throw ExceptionHelper.ThrowRegistrationOnly();
    }
}