using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;
using Hattem.CQRS.Errors;

namespace Hattem.CQRS
{
    public interface ICacheStorage
    {
        Task<ApiResponse<Unit>> Invalidate(string key, string region);

        Task<ApiResponse<Unit>> Invalidate(IEnumerable<(string Key, string Region)> keys);

        Task<ApiResponse<Unit>> InvalidateRegions(IEnumerable<string> regions);

        Task<ApiResponse<Unit>> InvalidateRegions(string region);

        Task<ApiResponse<Unit>> InvalidateRegions(string region1, string region2);

        Task<ApiResponse<Unit>> InvalidateRegions(params string[] regions);

        Task<ApiResponse<Unit>> Cache<T>(
            string key,
            string region,
            T value,
            TimeSpan? relativeExpiration
        );

        Task<ApiResponse<Unit>> GetValue<T>(string key, string region);
    }

    internal sealed class NoOpCacheStorage : ICacheStorage
    {
        private readonly Task<ApiResponse<Unit>> _cacheNotFoundError =
            ApiResponse
                .Error(CacheNotFoundError.Default)
                .AsTask();

        public Task<ApiResponse<Unit>> Invalidate(string key, string region)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> Invalidate(IEnumerable<(string Key, string Region)> keys)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> InvalidateRegions(IEnumerable<string> regions)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> InvalidateRegions(string region)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> InvalidateRegions(string region1, string region2)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> InvalidateRegions(params string[] regions)
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> Cache<T>(
            string key,
            string region,
            T value,
            TimeSpan? relativeExpiration
        )
        {
            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> GetValue<T>(string key, string region)
        {
            return _cacheNotFoundError;
        }
    }
}