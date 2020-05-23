using System;

namespace Hattem.CQRS.Queries
{
    public readonly struct QueryExecutionContext<TConnection, TResult>
        where TConnection : IHattemConnection
    {
        public IQueryHandler<TConnection, IQuery<TResult>, TResult> Handler { get; }

        public TConnection Connection { get; }

        public IQuery<TResult> Query { get; }

        public QueryExecutionContext(
            IQueryHandler<TConnection, IQuery<TResult>, TResult> handler,
            TConnection connection,
            IQuery<TResult> query
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }
    }

    public static class QueryExecutionContext
    {
        public static QueryExecutionContext<TConnection, TResult> Create<TConnection, TResult>(
            TConnection connection,
            IQueryHandler<TConnection, IQuery<TResult>, TResult> handler,
            IQuery<TResult> query
        )
            where TConnection : IHattemConnection
        {
            return new QueryExecutionContext<TConnection, TResult>(handler, connection, query);
        }
    }
}
