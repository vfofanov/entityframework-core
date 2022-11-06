using System;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    [Flags]
    public enum EntityFrameworkDefinitionReaderOptions
    {
        None = 0,
        ExcludeIgnoredProperties = 0x01,
        ExcludeScalarProperties = 0x02,
        ExcludeNavigationProperties = 0x04,
        ExcludeAbstractEntities = 0x08,
        ExcludeShadowProperties = 0x10
    }
}