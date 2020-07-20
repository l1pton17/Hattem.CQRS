using System;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Sql
{
    public interface ISqlHattemSessionFactory<out TSession> : IHattemSessionFactory<TSession>
        where TSession : class, IHattemSession, IHattemConnection
    {
    }

    public abstract class SqlHattemSessionFactory<TSession> : ISqlHattemSessionFactory<TSession>
        where TSession : class, IHattemSession, IHattemConnection
    {
        private readonly INotificationPublisher<TSession> _notificationPublisher;
        private readonly ICommandProcessorFactory<TSession> _commandProcessorFactory;
        private readonly IQueryProcessorFactory<TSession> _queryProcessorFactory;

        protected SqlHattemSessionFactory(
            ICommandProcessorFactory<TSession> commandProcessorFactory,
            INotificationPublisher<TSession> notificationPublisher,
            IQueryProcessorFactory<TSession> queryProcessorFactory
        )
        {
            _commandProcessorFactory = commandProcessorFactory ?? throw new ArgumentNullException(nameof(commandProcessorFactory));
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _queryProcessorFactory = queryProcessorFactory ?? throw new ArgumentNullException(nameof(queryProcessorFactory));
        }

        public TSession Create()
        {
            return Create(
                _notificationPublisher,
                _commandProcessorFactory,
                _queryProcessorFactory);
        }

        protected abstract TSession Create(
            INotificationPublisher<TSession> notificationPublisher,
            ICommandProcessorFactory<TSession> commandProcessorFactory,
            IQueryProcessorFactory<TSession> queryProcessorFactory);
    }
}
