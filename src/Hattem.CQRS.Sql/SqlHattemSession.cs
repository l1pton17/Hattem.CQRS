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
    public interface ISqlHattemSession :
        IHattemSession,
        IHattemConnection,
#if !NET461 && !NETSTANDARD2_0
        IAsyncDisposable,
#endif
        IDisposable
    {
        DbConnection Connection { get; }

        Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

        void Commit();

        void Rollback();

#if !NETSTANDARD2_0
        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);
#endif
    }

    public sealed class SqlHattemSession : ISqlHattemSession
    {
        private readonly INotificationPublisher<SqlHattemSession> _notificationPublisher;
        private readonly ICommandProcessor<SqlHattemSession> _commandProcessor;
        private readonly IQueryProcessor<SqlHattemSession> _queryProcessor;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IsolationLevel? _isolationLevel;

        private TransactionState? _transactionState;
        private Task<DbConnection> _connectionTask;
        private DbConnection _connection;
        private DbTransaction _transaction;

        public DbConnection Connection => _connection ??= SetupConnectionCore();

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

        public Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection != null)
            {
                return _connectionTask ??= Task.FromResult(_connection);
            }

            return SetupConnectionCoreAsync(cancellationToken);
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
            SetTransactionState(TransactionState.Commit);

            _transaction?.Commit();
        }

        public void Rollback()
        {
            SetTransactionState(TransactionState.Rollback);

            _transaction?.Rollback();
        }

#if !NETSTANDARD2_0
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            SetTransactionState(TransactionState.Commit);

            return _transaction == null
                ? Task.CompletedTask
                : _transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            SetTransactionState(TransactionState.Rollback);

            return _transaction == null
                ? Task.CompletedTask
                : _transaction.RollbackAsync(cancellationToken);
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

        private DbConnection SetupConnectionCore()
        {
            _connection = _dbConnectionFactory.Create();

            _connection.Open();

            if (_isolationLevel.HasValue)
            {
                _transaction = _connection.BeginTransaction(_isolationLevel.Value);
            }

            return _connection;
        }

        private async Task<DbConnection> SetupConnectionCoreAsync(CancellationToken cancellationToken = default)
        {
            _connection = _dbConnectionFactory.Create();

            await _connection
                .OpenAsync(cancellationToken)
                .ConfigureAwait(false);

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
            }

            return _connection;
        }

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
