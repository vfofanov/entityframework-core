using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with source system id property
    /// </summary>
    public interface IEntityWithSourceSystemId
    {
        /// <summary>
        /// Generate source system id
        /// </summary>
        /// <returns></returns>
        string GenerateSourceSystemId();

        string SourceSystemId => throw ExceptionHelper.ThrowRegistrationOnly();
    }
}