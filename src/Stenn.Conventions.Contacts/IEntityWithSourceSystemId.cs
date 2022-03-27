using System;

namespace Stenn.Conventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Created
    /// </summary>
    public interface IEntityWithSourceSystemId
    {
        /// <summary>
        /// Generate source system id
        /// </summary>
        /// <returns></returns>
        string GenerateSourceSystemId();
        
        string SourceSystemId => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
    }
}