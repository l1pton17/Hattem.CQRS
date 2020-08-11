using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Sql
{
    public interface ISqlNotificationHandler<in TDomainEvent> : INotificationHandler<SqlHattemSession, TDomainEvent>
        where TDomainEvent : INotification
    {
    }
}
