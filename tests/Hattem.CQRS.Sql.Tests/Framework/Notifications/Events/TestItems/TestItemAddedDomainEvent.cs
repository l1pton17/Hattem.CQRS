using System;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Notifications.Events.TestItems
{
    public sealed class TestItemAddedDomainEvent : INotification
    {
        public TestItem TestItem { get; }

        public TestItemAddedDomainEvent(TestItem testItem)
        {
            TestItem = testItem ?? throw new ArgumentNullException(nameof(testItem));
        }
    }
}
