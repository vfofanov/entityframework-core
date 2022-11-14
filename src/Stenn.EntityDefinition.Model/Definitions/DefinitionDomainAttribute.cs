﻿using Stenn.EntityDefinition.Contracts;

namespace Stenn.EntityDefinition.Model.Definitions
{
    /// <summary>
    /// Definition remark attribute
    /// </summary>
    public sealed class DefinitionDomainAttribute : DefinitionAttribute
    {
        /// <inheritdoc />
        public DefinitionDomainAttribute(Domain value) 
            : base("Domain", value)
        {
        }
    }
}