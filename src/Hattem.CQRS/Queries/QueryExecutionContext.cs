using System;

namespace Hattem.CQRS.Queries
{
    public readonly struct QueryExecutionContext<TConnection, TQuery, TResult>
        where TConnection : IHattemConnection
        where TQuery : IQuery<TResult>
    {
        public IQueryHandler<TConnection, TQuery, TResult> Handler { get; }

        public TConnection Connection { get; }

        public TQuery Query { get; }

        public QueryExecutionContext(
            IQueryHandler<TConnection, TQuery, TResult> handler,
            TConnection connection,
            in TQuery query
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Query = query;
        }
    }

    public static class QueryExecutionContext
    {
        public static QueryExecutionContext<TConnection, TQuery, TResult> Create<TConnection, TQuery, TResult>(
            TConnection connection,
            IQueryHandler<TConnection, TQuery, TResult> handler,
            TQuery query
        )
            where TConnection : IHattemConnection
            where TQuery : IQuery<TResult>
        {
            return new QueryExecutionContext<TConnection, TQuery, TResult>(handler, connection, in query);
        }
    }
}
