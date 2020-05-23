using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications.Handlers
{
    public abstract class OrderTrackerNotificationHandler : INotificationHandlerMock<OrderTrackerNotification>
    {
        public NotificationHandlerOptions Options { get; }

        protected OrderTrackerNotificationHandler(int order)
        {
            Options = NotificationHandlerOptions.Create(order: order);
        }

        public Task<ApiResponse<Unit>> Handle(HattemSessionMock session, OrderTrackerNotification notification)
        {
            notification.ExecutedHandlers.Add(GetType());

            return ApiResponse.OkAsync();
        }
    }

    public sealed class FirstOrderTrackerNotificationHandler : OrderTrackerNotificationHandler
    {
        public FirstOrderTrackerNotificationHandler()
            : base(1)
        {
        }
    }

    public sealed class SecondOrderTrackerNotificationHandler : OrderTrackerNotificationHandler
    {
        public SecondOrderTrackerNotificationHandler()
            : base(2)
        {
        }
    }

    public sealed class ThirdOrderTrackerNotificationHandler : OrderTrackerNotificationHandler
    {
        public ThirdOrderTrackerNotificationHandler()
            : base(2)
        {
        }
    }
}
