using System;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications
{
    public sealed class TestNotification : INotification
    {
        public Guid Data { get; } = Guid.NewGuid();
    }
}
