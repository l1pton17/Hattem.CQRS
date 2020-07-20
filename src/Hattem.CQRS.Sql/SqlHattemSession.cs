using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS.Sql
{
    public sealed class SqlHattemSession : IHattemSession, IHattemConnection
    {
        private readonly INotificationPublisher<SqlHattemSession> _notificationPublisher;

        public IQueryProcessor QueryProcessor { get; }

        public ICommandProcessor CommandProcessor { get; }

        protected SqlHattemSession(
            INotificationPublisher<SqlHattemSession> notificationPublisher,
            ICommandProcessorFactory<SqlHattemSession> commandProcessorFactory,
            IQueryProcessorFactory<SqlHattemSession> queryProcessorFactory)
        {
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
        }

        public Task<ApiResponse<Unit>> PublishNotification<T>(T notification)
            where T : INotification
        {
            return _notificationPublisher.Publish(this, notification);
        }
    }
}
