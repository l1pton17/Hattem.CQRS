using System;
using Hattem.CQRS.Notifications;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Notifications.Events.TestItems
{
    public sealed class TestItemAddedStructDomainEvent : INotification
    {
        public TestItem TestItem { get; }

        public TestItemAddedStructDomainEvent(TestItem testItem)
        {
            TestItem = testItem ?? throw new ArgumentNullException(nameof(testItem));
        }
    }
}
