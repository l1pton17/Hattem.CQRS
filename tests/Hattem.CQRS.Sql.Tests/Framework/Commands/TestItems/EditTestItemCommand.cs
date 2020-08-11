using System;
using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.CQRS.Commands;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems
{
    public sealed class EditTestItemCommand : ICommand
    {
        public TestItem Value { get; }

        public EditTestItemCommand(TestItem value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public sealed class EditTestItemCommandHandler : ISqlCommandHandler<EditTestItemCommand>
    {
        private static readonly string _script = $@"
UPDATE test_items
SET value = @{nameof(TestItem.Value)}
WHERE id = @{nameof(TestItem.Id)}
;";

        public async Task<ApiResponse<Unit>> Execute(SqlHattemSession connection, EditTestItemCommand command)
        {
            await connection.Connection.ExecuteAsync(_script, command.Value);

            return ApiResponse.Ok();
        }
    }
}
