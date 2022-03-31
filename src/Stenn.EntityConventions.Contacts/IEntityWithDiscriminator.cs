namespace Stenn.EntityConventions.Contacts
{
    /// <summary>
    /// Entity with discriminator for inheritance
    /// </summary>
    public interface IEntityWithDiscriminator<out T> : IEntityWithDiscriminator
    {
        T Discriminator => throw ExceptionHelper.ThrowRegistrationOnly();
    }

    /// <summary>
    /// Entity with discriminator for inheritance
    /// </summary>
    public interface IEntityWithDiscriminator
    {
    }
}