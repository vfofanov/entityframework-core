using System;

namespace Stenn.Conventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Modified
    /// </summary>
    public interface IUpdateAuditedEntity
    {
        DateTime ModifiedAt => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
    }
}