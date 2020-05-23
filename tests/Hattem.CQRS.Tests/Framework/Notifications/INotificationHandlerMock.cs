using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications
{
    public interface INotificationHandlerMock<in TDomainEvent> : INotificationHandler<HattemSessionMock, TDomainEvent>
        where TDomainEvent : INotification
    {
    }
}
