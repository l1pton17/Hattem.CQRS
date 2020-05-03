using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;

namespace Hattem.CQRS.Notifications
{
    public interface INotificationPublisher<in TSession>
        where TSession : IHattemSession
    {
        Task<ApiResponse<Unit>> Publish<TNotification>(TSession session, TNotification notification)
            where TNotification : INotification;
    }

    internal sealed class NotificationPublisher<TSession, TConnection> : INotificationPublisher<TSession>
        where TSession : IHattemSession
        where TConnection : IHattemConnection
    {
        private readonly IHandlerProvider<TSession, TConnection> _handlerProvider;

        public NotificationPublisher(IHandlerProvider<TSession, TConnection> handlerProvider)
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
        }

        public Task<ApiResponse<Unit>> Publish<TNotification>(TSession session, TNotification notification)
            where TNotification : INotification
        {
            var handlers = _handlerProvider.GetNotificationHandlers<TNotification>();

            return handlers.ForEach(
                handler => handler
                    .Handle(session, notification)
                    .Catch()
                    .Unwrap());
        }
    }
}