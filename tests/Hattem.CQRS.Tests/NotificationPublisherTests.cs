using System.Threading.Tasks;
using Hattem.CQRS.Tests.Framework;
using Hattem.CQRS.Tests.Framework.Errors;
using Hattem.CQRS.Tests.Framework.Notifications;
using Hattem.CQRS.Tests.Framework.Notifications.Handlers;
using Hattem.CQRS.Tests.Framework.Notifications.Pipeline;
using Xunit;

namespace Hattem.CQRS.Tests
{
    [CategoryTrait("NotificationPublisher")]
    public sealed class NotificationPublisherTests : TestsBase
    {
        [Fact(DisplayName = "Should pass notification to context")]
        public async Task Publish_PassNotificationToContext()
        {
            var session = CreateSession();
            var notification = TestNotification.New();

            await session.PublishNotification(notification);

            CatchNotificationPipelineStep.AssertNotificationContextCaptured<TestNotification>(context => context.Notification.Equals(notification));
        }

        [Fact(DisplayName = "Should pass handler to context")]
        public async Task Publish_PassHandlerToContext()
        {
            var session = CreateSession();
            var notification = TestNotification.New();

            await session.PublishNotification(notification);

            CatchNotificationPipelineStep.AssertNotificationContextCaptured<TestNotification>(context => context.Handler is TestNotificationHandler);
        }

        [Fact(DisplayName = "Should pass session to context")]
        public async Task Publish_PassSessionToContext()
        {
            var session = CreateSession();
            var notification = TestNotification.New();

            await session.PublishNotification(notification);

            CatchNotificationPipelineStep.AssertNotificationContextCaptured<TestNotification>(context => context.Session.Id == session.Id);
        }

        [Fact(DisplayName = "Should execute handler")]
        public async Task Publish_ExecuteHandlers()
        {
            var session = CreateSession();
            var notification = TestNotification.New();

            await session.PublishNotification(notification);

            Assert.Contains(notification.Data, TestNotificationHandler.ExecutedNotifications);
        }

        [Fact(DisplayName = "Should return error if required handler returns an error")]
        public async Task Publish_RequiredHandlerReturnsError_ReturnsError()
        {
            var session = CreateSession();
            var notification = new ErrorNotification();

            var response = await session.PublishNotification(notification);

            Assert.True(response.HasErrors);
            Assert.Equal(ErrorCodes.Test, response.Error.Code);
        }

        [Fact(DisplayName = "Should return ok if not required handler returns an error")]
        public async Task Publish_NotRequiredHandlerReturnsError_ReturnsOk()
        {
            var session = CreateSession();
            var notification = new NotRequiredErrorNotification();

            var response = await session.PublishNotification(notification);

            Assert.True(response.IsOk);
        }

        [Fact(DisplayName = "Should execute handlers by order")]
        public async Task Publish_ExecuteHandlersAccordingToOrder()
        {
            var session = CreateSession();
            var notification = new OrderTrackerNotification();
            var response = await session.PublishNotification(notification);

            Assert.True(response.IsOk);
            Assert.Equal(3, notification.ExecutedHandlers.Count);
            Assert.Equal(typeof(FirstOrderTrackerNotificationHandler), notification.ExecutedHandlers[0]);
            Assert.Equal(typeof(SecondOrderTrackerNotificationHandler), notification.ExecutedHandlers[1]);
            Assert.Equal(typeof(ThirdOrderTrackerNotificationHandler), notification.ExecutedHandlers[2]);
        }
    }
}
