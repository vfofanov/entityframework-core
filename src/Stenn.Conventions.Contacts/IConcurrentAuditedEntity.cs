using System;

namespace Stenn.Conventions.Contacts
{
    /// <summary>
    /// Entity with concurrent row version property
    /// </summary>
    public interface IConcurrentAuditedEntity
    {
        byte[] RowVersion => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
    }
}