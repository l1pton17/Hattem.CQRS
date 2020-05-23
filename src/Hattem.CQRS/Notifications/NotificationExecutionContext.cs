using System;

namespace Hattem.CQRS.Notifications
{
    public readonly struct NotificationExecutionContext<TSession, TNotification>
        where TSession : IHattemSession
        where TNotification : INotification
    {
        public INotificationHandler<TSession, TNotification> Handler { get; }

        public TSession Session { get; }

        public TNotification Notification { get; }

        public NotificationExecutionContext(
            TSession session,
            INotificationHandler<TSession, TNotification> handler,
            TNotification notification
        )
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Session = session ?? throw new ArgumentNullException(nameof(session));
            Notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
    }

    public static class NotificationExecutionContext
    {
        public static NotificationExecutionContext<TSession, TNotification> Create<TSession, TNotification>(
            TSession session,
            INotificationHandler<TSession, TNotification> handler,
            TNotification notification
        )
            where TSession : IHattemSession
            where TNotification : INotification
        {
            return new NotificationExecutionContext<TSession, TNotification>(session, handler, notification);
        }
    }
}
