using System;
using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.Api.Fluent;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Sql.Tests.Framework.Models;
using Hattem.CQRS.Sql.Tests.Framework.Notifications.Events.TestItems;

namespace Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems
{
    public sealed class AddTestItemCommand : ICommand<TestItem>
    {
        public TestItem Value { get; }

        public AddTestItemCommand(TestItem value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public sealed class AddTestItemCommandHandler : ISqlCommandHandler<AddTestItemCommand, TestItem>
    {
        private static readonly string _script = $@"
INSERT INTO test_items (
    value
)
VALUES (
    @{nameof(TestItem.Value)}
)
RETURNING id
;";

        public async Task<ApiResponse<TestItem>> Execute(SqlHattemSession connection, AddTestItemCommand command)
        {
            command.Value.Id = await connection.Connection.ExecuteScalarAsync<int>(_script, command.Value);

            return await connection
                .PublishNotification(new TestItemAddedDomainEvent(command.Value))
                .Return(command.Value);
        }
    }
}
