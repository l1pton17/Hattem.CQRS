using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Tests.Framework
{
    public sealed class HattemSessionMock : IHattemSession, IHattemConnection
    {
        private readonly INotificationPublisher<HattemSessionMock> _notificationPublisher;

        public IQueryProcessor QueryProcessor { get; }

        public ICommandProcessor CommandProcessor { get; }

        public HattemSessionMock(
            INotificationPublisher<HattemSessionMock> notificationPublisher,
            ICommandProcessorFactory<HattemSessionMock> commandProcessorFactory,
            IQueryProcessorFactory<HattemSessionMock> queryProcessorFactory)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            CommandProcessor = commandProcessorFactory?.Create(this) ?? throw new ArgumentNullException(nameof(commandProcessorFactory));
            QueryProcessor = queryProcessorFactory?.Create(this) ?? throw new ArgumentNullException(nameof(queryProcessorFactory));
        }

        public Task<ApiResponse<Unit>> PublishNotification<T>(T notification)
            where T : INotification
        {
            return _notificationPublisher.Publish(this, notification);
        }
    }
}