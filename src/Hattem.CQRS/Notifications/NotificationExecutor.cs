using System;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;
using Hattem.CQRS.Notifications.Pipeline;

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
        private readonly INotificationExecutor<TSession> _executor;

        public NotificationPublisher(
            IHandlerProvider<TSession, TConnection> handlerProvider,
            INotificationExecutor<TSession> executor
        )
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<ApiResponse<Unit>> Publish<TNotification>(TSession session, TNotification notification)
            where TNotification : INotification
        {
            var handlers = _handlerProvider.GetNotificationHandlers<TNotification>();

            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var context = NotificationExecutionContext.Create(session, handler, notification);

                var notifyResponse = await _executor.Handle(context).Catch().Unwrap().ConfigureAwait(false);

                if (notifyResponse.HasErrors && handler.Options.IsRequired)
                {
                    return notifyResponse;
                }
            }

            return ApiResponse.Ok();
        }
    }
}
