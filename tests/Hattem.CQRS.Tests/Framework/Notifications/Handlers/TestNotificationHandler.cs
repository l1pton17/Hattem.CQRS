using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications.Handlers
{
    public sealed class TestNotificationHandler : INotificationHandlerMock<TestNotification>
    {
        public static ConcurrentBag<Guid> ExecutedNotifications { get; } = new ConcurrentBag<Guid>();

        public NotificationHandlerOptions Options { get; } = NotificationHandlerOptions.Create(isRequired: true);

        public Task<ApiResponse<Unit>> Handle(HattemSessionMock session, TestNotification notification)
        {
            ExecutedNotifications.Add(notification.Data);

            return ApiResponse.OkAsync();
        }
    }
}
