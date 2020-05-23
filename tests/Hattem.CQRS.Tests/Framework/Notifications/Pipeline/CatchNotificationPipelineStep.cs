using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Notifications.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests.Framework.Notifications.Pipeline
{
    public sealed class CatchNotificationPipelineStep : INotificationPipelineStep
    {
        public Task<ApiResponse<Unit>> Handle<TSession, TNotification>(
            NotificationDelegate<TSession, TNotification> next,
            NotificationExecutionContext<TSession, TNotification> context)
            where TSession : IHattemSession
            where TNotification : INotification
        {
            NotificationCapturedContextStorage<TSession, TNotification>.CapturedContext.Add(context);

            return next(context);
        }

        public static void AssertNotificationContextCaptured<TNotification>(Func<NotificationExecutionContext<HattemSessionMock, TNotification>, bool> predicate)
            where TNotification : INotification
        {
            var contains = NotificationCapturedContextStorage<HattemSessionMock, TNotification>.CapturedContext.Any(predicate);

            Assert.True(contains);
        }

        private static class NotificationCapturedContextStorage<TSession, TNotification>
            where TSession : IHattemSession
            where TNotification : INotification
        {
            public static ConcurrentBag<NotificationExecutionContext<TSession, TNotification>> CapturedContext { get; } =
                new ConcurrentBag<NotificationExecutionContext<TSession, TNotification>>();
        }
    }
}
