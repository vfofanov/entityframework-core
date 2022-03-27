using System;

namespace Stenn.Conventions.Contacts
{
    /// <summary>
    /// Entity with manual concurrent row version property
    /// </summary>
    public interface IManualConcurrentAuditedEntity
    {
        byte[] ManualRowVersion
        {
            get => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
            set => throw new NotSupportedException("This property exists for EF declaration only. For usage explicit declare it in inheritor");
        }
    }
}