using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Modified
    /// </summary>
    public interface ISoftDeleteEntity
    {
        bool IsDeleted => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
        DateTime? Deleted => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
    }
}