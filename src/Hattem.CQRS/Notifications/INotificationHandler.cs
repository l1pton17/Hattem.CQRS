using System.Threading.Tasks;
using Hattem.Api;

namespace Hattem.CQRS.Notifications
{
    public interface IHasNotificationHandlerOptions
    {
        /// <summary>
        /// Options
        /// </summary>
        NotificationHandlerOptions Options { get; }
    }

    /// <summary>
    /// Base interface for domain event handler. Do not use explicit.
    /// </summary>
    public interface INotificationHandler<in TSession, in TNotification> : IHasNotificationHandlerOptions
        where TNotification : INotification
        where TSession : IHattemSession
    {
        /// <summary>
        /// Handle domain event
        /// </summary>
        /// <param name="session"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task<ApiResponse<Unit>> Handle(TSession session, TNotification notification);
    }
}
