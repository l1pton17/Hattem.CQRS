﻿using System;
using Hattem.CQRS.Queries.Pipeline;

namespace Hattem.CQRS.Queries
{
    public interface IQueryProcessorFactory<in TConnection>
        where TConnection : IHattemConnection
    {
        IQueryProcessor Create(TConnection connection);
    }

    internal sealed class QueryProcessorFactory<TSession, TConnection> : IQueryProcessorFactory<TConnection>
        where TConnection : IHattemConnection
        where TSession : IHattemSession
    {
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;
        private readonly IQueryExecutor<TConnection> _queryExecutor;

        public QueryProcessorFactory(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            IQueryExecutor<TConnection> queryExecutor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        }

        public IQueryProcessor Create(TConnection connection)
        {
            return new QueryProcessor<TSession, TConnection>(
                connection,
                _handlerProvider,
                _queryExecutor);
        }
    }
}