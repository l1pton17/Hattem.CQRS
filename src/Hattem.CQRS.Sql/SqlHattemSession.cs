using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Sql
{
    public sealed class SqlHattemSession :
        IHattemSession,
        IHattemConnection,
#if !NET461 && !NETSTANDARD2_0
        IAsyncDisposable,
#endif
        IDisposable
    {
        private readonly INotificationPublisher<SqlHattemSession> _notificationPublisher;
        private readonly ICommandProcessor<SqlHattemSession> _commandProcessor;
        private readonly IQueryProcessor<SqlHattemSession> _queryProcessor;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IsolationLevel? _isolationLevel;

        private TransactionState? _transactionState;
        private DbConnection _connection;
        private DbTransaction _transaction;

        public SqlHattemSession(
            INotificationPublisher<SqlHattemSession> notificationPublisher,
            ICommandProcessor<SqlHattemSession> commandProcessor,
            IQueryProcessor<SqlHattemSession> queryProcessor,
            IDbConnectionFactory dbConnectionFactory,
            IsolationLevel? isolationLevel
        )
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _isolationLevel = isolationLevel;
        }

        public ValueTask<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            return _connection != null
                ? new ValueTask<DbConnection>(_connection)
                : new ValueTask<DbConnection>(GetConnectionCoreAsync(cancellationToken));
        }

        public Task<ApiResponse<Unit>> PublishNotification<T>(T notification)
            where T : INotification
        {
            return _notificationPublisher.Publish(this, notification);
        }

        public Task<ApiResponse<TResult>> ProcessQuery<TResult>(IQuery<TResult> query)
        {
            return _queryProcessor.Process(this, query);
        }

        public Task<ApiResponse<TResult>> ProcessStructQuery<TQuery, TResult>(in TQuery query, Returns<TResult> returnsType)
            where TQuery : struct, IQuery<TResult>
        {
            return _queryProcessor.ProcessStruct(this, query, returnsType);
        }

        public Task<ApiResponse<Unit>> ExecuteCommand<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            return _commandProcessor.Execute(this, command);
        }

        public Task<ApiResponse<TReturn>> ExecuteCommandAndReturn<TReturn>(ICommand<TReturn> command)
        {
            return _commandProcessor.ExecuteAndReturn(this, command);
        }

        public Task<ApiResponse<TReturn>> ExecuteStructCommandAndReturn<TCommand, TReturn>(in TCommand command, Returns<TReturn> returnsType)
            where TCommand : struct, ICommand<TReturn>
        {
            return _commandProcessor.ExecuteStructAndReturn(this, command, returnsType);
        }

        public void Commit()
        {
            if (_transaction == null)
            {
                SetTransactionState(TransactionState.Commit);
            }

            _transaction.Commit();

            SetTransactionState(TransactionState.Commit);
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                SetTransactionState(TransactionState.Rollback);
            }

            _transaction.Rollback();

            SetTransactionState(TransactionState.Rollback);
        }

#if !NETSTANDARD2_0
        public ValueTask CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                SetTransactionState(TransactionState.Commit);

                return default;
            }

            return new ValueTask(CommitCoreAsync(cancellationToken));
        }

        public ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                SetTransactionState(TransactionState.Rollback);

                return default;
            }

            return new ValueTask(RollbackCoreAsync(cancellationToken));
        }
#endif

        public void Dispose()
        {
            try
            {
                if (_transaction != null && !_transactionState.HasValue)
                {
                    _transaction.Rollback();

                    SetTransactionState(TransactionState.Rollback);
                }
            }
            finally
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }

            _connection?.Dispose();
        }

#if !NETSTANDARD2_0
        public ValueTask DisposeAsync()
        {
            if (_connection == null)
            {
                return default;
            }

            return new ValueTask(DisposeCoreAsync());
        }

        private async Task DisposeCoreAsync()
        {
            try
            {
                if (_transaction != null && !_transactionState.HasValue)
                {
                    await RollbackAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync().ConfigureAwait(false);
                }

                if (_connection != null)
                {
                    await _connection.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
#endif

        private async Task<DbConnection> GetConnectionCoreAsync(CancellationToken cancellationToken = default)
        {
            _connection = _dbConnectionFactory.Create();

            if (_isolationLevel.HasValue)
            {
#if !NETSTANDARD2_0
                _transaction = await _connection
                    .BeginTransactionAsync(_isolationLevel.Value, CancellationToken.None)
                    .ConfigureAwait(false);
#else
                _transaction = _connection
                    .BeginTransaction(_isolationLevel.Value);
#endif

                _connection = _transaction.Connection;
            }

            await _connection
                .OpenAsync(cancellationToken)
                .ConfigureAwait(false);

            return _connection;
        }

#if !NETSTANDARD2_0
        private async Task RollbackCoreAsync(CancellationToken cancellationToken = default)
        {
            await _transaction
                .RollbackAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task CommitCoreAsync(CancellationToken cancellationToken = default)
        {
            await _transaction
                .CommitAsync(cancellationToken)
                .ConfigureAwait(false);

            SetTransactionState(TransactionState.Commit);
        }
#endif

        private void SetTransactionState(TransactionState value)
        {
            if (_transactionState.HasValue)
            {
                throw new InvalidOperationException("Operation already completed");
            }

            _transactionState = value;
        }

        private enum TransactionState
        {
            Commit,
            Rollback
        }
    }
}
