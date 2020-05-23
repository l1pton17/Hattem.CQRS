using System.Threading.Tasks;
using Hattem.Api;
using Hattem.Api.Fluent;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Tests.Framework.Errors;

namespace Hattem.CQRS.Tests.Framework.Notifications.Handlers
{
    public sealed class NotRequiredErrorNotificationHandler : INotificationHandlerMock<NotRequiredErrorNotification>
    {
        public NotificationHandlerOptions Options { get; } = NotificationHandlerOptions.Create(isRequired: false);

        public Task<ApiResponse<Unit>> Handle(HattemSessionMock session, NotRequiredErrorNotification notification)
        {
            var error = new TestError();

            return ApiResponse
                .Error(error)
                .AsTask();
        }
    }
}
