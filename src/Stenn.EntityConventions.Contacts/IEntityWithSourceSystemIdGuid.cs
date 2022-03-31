﻿using System;

namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with creation audited property Created
    /// </summary>
    public interface IEntityWithSourceSystemIdGuid : IEntityWithSourceSystemId
    {
        Guid Id { get; }

        string IEntityWithSourceSystemId.GenerateSourceSystemId()
        {
            return Id.ToString("N");
        }
    }
}