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
        private readonly ICommandProcessor<HattemSessionMock> _commandProcessor;
        private readonly IQueryProcessor<HattemSessionMock> _queryProcessor;

        public HattemSessionFactoryMock(
            ICommandProcessorFactory<HattemSessionMock> commandProcessorFactory,
            INotificationPublisher<HattemSessionMock> notificationPublisher,
            IQueryProcessorFactory<HattemSessionMock> queryProcessorFactory
        )
        {
            _commandProcessor = commandProcessorFactory?.Create() ?? throw new ArgumentNullException(nameof(commandProcessorFactory));
            _queryProcessor = queryProcessorFactory?.Create() ?? throw new ArgumentNullException(nameof(queryProcessorFactory));
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
        }

        public HattemSessionMock Create()
        {
            return new HattemSessionMock(
                _notificationPublisher,
                _commandProcessor,
                _queryProcessor);
        }
    }
}
