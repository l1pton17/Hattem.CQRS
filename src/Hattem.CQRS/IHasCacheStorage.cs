namespace Hattem.CQRS
{
    public interface IHasCacheStorage
    {
        ICacheStorage CacheStorage { get; }
    }
}
