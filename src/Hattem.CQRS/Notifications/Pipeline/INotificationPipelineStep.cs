using System;
using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Notifications.Pipeline
{
    public delegate Task<ApiResponse<Unit>> NotificationDelegate<TSession, TNotification>(NotificationExecutionContext<TSession, TNotification> context)
        where TSession : IHattemSession
        where TNotification : INotification;

    public interface INotificationPipelineStep
    {
        Task<ApiResponse<Unit>> Handle<TSession, TNotification>(
            NotificationDelegate<TSession, TNotification> next,
            NotificationExecutionContext<TSession, TNotification> context)
            where TSession : IHattemSession
            where TNotification : INotification;
    }
}
