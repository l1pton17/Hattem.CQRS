using System;
using System.Data;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;

namespace Hattem.CQRS.Sql.Extensions
{
    public static class SqlHattemSessionFactoryExtensions
    {
#if !NETSTANDARD2_0
        public static async Task<ApiResponse<T>> Execute<T>(
            this ISqlHattemSessionFactory sessionFactory,
            Func<ISqlHattemSession, Task<ApiResponse<T>>> handler,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        )
        {
            await using var session = sessionFactory.Create(isolationLevel);

            return await handler(session)
                .OnError(ErrorPredicate.Any(), _ => session.RollbackAsync())
                .OnSuccess(_ => session.CommitAsync());
        }
#endif

#if NETSTANDARD2_0
        public static async Task<ApiResponse<T>> Execute<T>(
            this ISqlHattemSessionFactory sessionFactory,
            Func<ISqlHattemSession, Task<ApiResponse<T>>> handler,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        )
        {
            using var session = sessionFactory.Create(isolationLevel);

            return await handler(session)
                .OnError(ErrorPredicate.Any(), _ => session.Rollback())
                .OnSuccess(_ => session.Commit());
        }
#endif

#if !NETSTANDARD2_0
        public static async Task<ApiResponse<T>> Fetch<T>(
            this ISqlHattemSessionFactory sessionFactory,
            Func<ISqlHattemSession, Task<ApiResponse<T>>> handler
        )
        {
            await using var session = sessionFactory.Create();

            return await handler(session)
                .OnError(ErrorPredicate.Any(), _ => session.RollbackAsync())
                .OnSuccess(_ => session.CommitAsync());
        }
#endif

#if NETSTANDARD2_0
        public static async Task<ApiResponse<T>> Fetch<T>(
            this ISqlHattemSessionFactory sessionFactory,
            Func<ISqlHattemSession, Task<ApiResponse<T>>> handler
        )
        {
            using var session = sessionFactory.Create();

            return await handler(session)
                .OnError(ErrorPredicate.Any(), _ => session.Rollback())
                .OnSuccess(_ => session.Commit());
        }
#endif
    }
}
