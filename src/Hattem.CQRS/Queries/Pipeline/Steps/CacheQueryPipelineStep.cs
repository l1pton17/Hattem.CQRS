﻿using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;

namespace Hattem.CQRS.Queries.Pipeline.Steps
{
    internal sealed class CacheQueryPipelineStep : IQueryPipelineStep
    {
        private readonly ICacheStorage _cacheStorage;

        public CacheQueryPipelineStep(ICacheStorage cacheStorage)
        {
            _cacheStorage = cacheStorage ?? throw new ArgumentNullException(nameof(cacheStorage));
        }

        public Task<ApiResponse<TResult>> Process<TConnection, TQuery, TResult>(
            Func<QueryExecutionContext<TConnection, TQuery, TResult>, Task<ApiResponse<TResult>>> next,
            QueryExecutionContext<TConnection, TQuery, TResult> context
        )
            where TConnection : IHattemConnection
            where TQuery : IQuery<TResult>
        {
            if (context.Handler is ICachedQueryHandler<TQuery> cachedQueryHandler)
            {
                var cacheKeyOption = cachedQueryHandler.GetCacheKeyOrDefault(context.Query);

                if (cacheKeyOption.HasValue)
                {
                    var cacheKey = cacheKeyOption.Value;

                    return _cacheStorage
                        .GetValue<TResult>(
                            cacheKey.CacheKey,
                            cacheKey.CacheRegion)
                        .IfError(
                            ErrorPredicate.ByCode(CQRSErrorCodes.CacheNotFound),
                            _ => next(context)
                                .Filter(
                                    result => _cacheStorage.Cache(
                                        cacheKey.CacheKey,
                                        cacheKey.CacheRegion,
                                        result,
                                        cacheKey.Expiration)));
                }
            }

            return next(context);
        }
    }
}
