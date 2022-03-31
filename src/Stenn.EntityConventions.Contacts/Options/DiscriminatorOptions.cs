using System;

namespace Stenn.EntityConventions.Contacts
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DiscriminatorOptions : StringPropertyOptions
    {
    }
}