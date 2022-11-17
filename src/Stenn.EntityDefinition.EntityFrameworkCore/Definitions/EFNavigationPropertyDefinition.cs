﻿using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFRelationNavigationPropertyDefinition<T> : EFPropertyDefinition<T>
    {
        /// <inheritdoc />
        public EFRelationNavigationPropertyDefinition(string name,
            Func<INavigation?, PropertyInfo?, T?, EntityDefinitionRow, PropertyDefinitionRow, DefinitionContext, T?> extract, Func<T, string>? convertToString = null)
            : base(name,
                (property, propertyInfo, parentValue, entityRow, row, context)  =>
                {
                    if (property is INavigation p && !p.TargetEntityType.IsOwned())
                    {
                        return extract(p, propertyInfo, parentValue, entityRow, row, context);
                    }
                    return default;
                }, convertToString)
        {
            if (extract == null)
            {
                throw new ArgumentNullException(nameof(extract));
            }
        }
    }
}