using System;

namespace Hattem.CQRS.Queries
{
    public struct QueryCacheKey
    {
        public string CacheKey { get; }

        public string CacheRegion { get; }

        public TimeSpan? Expiration { get; }

        public QueryCacheKey(
            string cacheKey,
            string cacheRegion,
            TimeSpan? expiration
        )
        {
            CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
            CacheRegion = cacheRegion;
            Expiration = expiration;
        }
    }
}
