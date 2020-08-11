using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems
{
    public readonly struct AddTestItemTableCommand : ICommand
    {
        public static readonly AddTestItemTableCommand Default = default;
    }

    public sealed class AddTestItemTableCommandHandler : ISqlCommandHandler<AddTestItemTableCommand>
    {
        private static readonly string _script = $@"
CREATE TABLE IF NOT EXISTS test_items (
    id        serial    PRIMARY KEY
   ,value     text      NOT NULL        
)
;";

        public async Task<ApiResponse<Unit>> Execute(SqlHattemSession connection, AddTestItemTableCommand command)
        {
            var sqlConnection = await connection.GetConnectionAsync();

            await sqlConnection.ExecuteAsync(_script);

            return ApiResponse.Ok();
        }
    }
}
