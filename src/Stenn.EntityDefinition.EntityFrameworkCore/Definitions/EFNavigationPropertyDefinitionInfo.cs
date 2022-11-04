using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFNavigationPropertyDefinitionInfo<T> : EFPropertyDefinitionInfo<T>
    {
        /// <inheritdoc />
        public EFNavigationPropertyDefinitionInfo(string name,
            Func<INavigation?, PropertyInfo?, IEFDefinitionExtractContext, T?> extract, Func<T, string>? convertToString = null)
            : base(name,
                (property, propertyInfo, context) =>
                {
                    if (property is INavigation p)
                    {
                        return extract(p, propertyInfo, context);
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