using System;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework
{
    public interface IHattemSessionFactoryMock : IHattemSessionFactory<HattemSessionMock>
    {
    }

    public sealed class HattemSessionFactoryMock : IHattemSessionFactoryMock
    {
        private readonly INotificationPublisher<HattemSessionMock> _notificationPublisher;
        private readonly ICommandProcessorFactory<HattemSessionMock> _commandProcessorFactory;
        private readonly IQueryProcessorFactory<HattemSessionMock> _queryProcessorFactory;

        public HattemSessionFactoryMock(
            ICommandProcessorFactory<HattemSessionMock> commandProcessorFactory,
            INotificationPublisher<HattemSessionMock> notificationPublisher,
            IQueryProcessorFactory<HattemSessionMock> queryProcessorFactory
        )
        {
            _commandProcessorFactory = commandProcessorFactory ?? throw new ArgumentNullException(nameof(commandProcessorFactory));
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _queryProcessorFactory = queryProcessorFactory ?? throw new ArgumentNullException(nameof(queryProcessorFactory));
        }

        public HattemSessionMock Create()
        {
            return new HattemSessionMock(
                _notificationPublisher,
                _commandProcessorFactory,
                _queryProcessorFactory);
        }
    }
}