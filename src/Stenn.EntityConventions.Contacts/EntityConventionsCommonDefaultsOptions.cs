namespace Stenn.EntityConventions.Contacts
{
    public sealed class EntityConventionsCommonDefaultsOptions
    {
        /// <summary>
        /// Default options for <see cref="IEntityWithSourceSystemId"/> entity convention
        /// </summary>
        public SourceSystemIdOptions SourceSystemId { get; } = new();
        /// <summary>
        /// Default options for <see cref="IEntityWithDiscriminator{T}"/> entity convention
        /// </summary>
        public DiscriminatorOptions Discriminator { get; } = new();
    }
}