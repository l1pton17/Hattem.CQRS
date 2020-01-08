namespace Hattem.CQRS.Queries
{
    /// <summary>
    /// Allow handler cache the results
    /// </summary>
    public interface ICachedQueryHandler<in TQuery>
    {
        /// <summary>
        /// Get cache key.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Cache key information or null if no cache should be provided</returns>
        QueryCacheKey? GetCacheKeyOrDefault(TQuery query);
    }
}