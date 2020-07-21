using System;
using System.Data;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Sql
{
    public interface ISqlHattemSessionFactory : IHattemSessionFactory<SqlHattemSession>
    {
        SqlHattemSession Create(IsolationLevel isolationLevel);
    }

    public sealed class SqlHattemSessionFactory : ISqlHattemSessionFactory
    {
        private readonly INotificationPublisher<SqlHattemSession> _notificationPublisher;
        private readonly ICommandProcessor<SqlHattemSession> _commandProcessor;
        private readonly IQueryProcessor<SqlHattemSession> _queryProcessor;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlHattemSessionFactory(
            ICommandProcessorFactory<SqlHattemSession> commandProcessorFactory,
            INotificationPublisher<SqlHattemSession> notificationPublisher,
            IQueryProcessorFactory<SqlHattemSession> queryProcessorFactory,
            IDbConnectionFactory dbConnectionFactory
        )
        {
            _commandProcessor = commandProcessorFactory?.Create() ?? throw new ArgumentNullException(nameof(commandProcessorFactory));
            _queryProcessor = queryProcessorFactory?.Create() ?? throw new ArgumentNullException(nameof(queryProcessorFactory));
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public SqlHattemSession Create()
        {
            return new SqlHattemSession(
                _notificationPublisher,
                _commandProcessor,
                _queryProcessor,
                _dbConnectionFactory,
                isolationLevel: null);
        }

        public SqlHattemSession Create(IsolationLevel isolationLevel)
        {
            return new SqlHattemSession(
                _notificationPublisher,
                _commandProcessor,
                _queryProcessor,
                _dbConnectionFactory,
                isolationLevel);
        }
    }
}
