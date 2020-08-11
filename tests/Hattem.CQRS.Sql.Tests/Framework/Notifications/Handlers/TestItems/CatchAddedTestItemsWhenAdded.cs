using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Sql.Tests.Framework.Models;
using Hattem.CQRS.Sql.Tests.Framework.Notifications.Events.TestItems;

namespace Hattem.CQRS.Sql.Tests.Framework.Notifications.Handlers.TestItems
{
    public sealed class CatchAddedTestItemsWhenAdded :
        ISqlNotificationHandler<TestItemAddedDomainEvent>,
        ISqlNotificationHandler<TestItemAddedStructDomainEvent>
    {
        public static ConcurrentBag<TestItem> CaughtItems { get; } = new ConcurrentBag<TestItem>();

        public NotificationHandlerOptions Options { get; } = NotificationHandlerOptions.Create(isRequired: true);

        public Task<ApiResponse<Unit>> Handle(SqlHattemSession session, TestItemAddedDomainEvent notification)
        {
            CaughtItems.Add(notification.TestItem);

            return ApiResponse.OkAsync();
        }

        public Task<ApiResponse<Unit>> Handle(SqlHattemSession session, TestItemAddedStructDomainEvent notification)
        {
            CaughtItems.Add(notification.TestItem);

            return ApiResponse.OkAsync();
        }
    }
}
