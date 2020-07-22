using System;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS.Tests.Framework.Notifications
{
    public readonly struct TestNotification : INotification
    {
        public Guid Data { get; }

        private TestNotification(Guid data)
        {
            Data = data;
        }

        public static TestNotification New() => new TestNotification(Guid.NewGuid());
    }
}
