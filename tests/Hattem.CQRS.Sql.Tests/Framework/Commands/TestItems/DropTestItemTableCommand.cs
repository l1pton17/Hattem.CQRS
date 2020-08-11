using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems
{
    public readonly struct DropTestItemTableCommand : ICommand
    {
        public static readonly DropTestItemTableCommand Default = default;
    }

    public sealed class DropTestItemTableCommandHandler : ISqlCommandHandler<DropTestItemTableCommand>
    {
        private static readonly string _script = $@"
DROP TABLE test_items;
;";

        public async Task<ApiResponse<Unit>> Execute(SqlHattemSession connection, DropTestItemTableCommand command)
        {
            await connection.Connection.ExecuteAsync(_script);

            return ApiResponse.Ok();
        }
    }
}
