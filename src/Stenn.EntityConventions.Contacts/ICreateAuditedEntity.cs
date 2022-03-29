using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Created
    /// </summary>
    public interface ICreateAuditedEntity
    {
        DateTime Created => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
    }
}