using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Queries.TestItems
{
    public readonly struct GetTestItemStructQuery : IQuery<TestItem>
    {
        public int Id { get; }

        public GetTestItemStructQuery(int id)
        {
            Id = id;
        }
    }

    public sealed class GetTestItemStructQueryHandler : ISqlQueryHandler<GetTestItemStructQuery, TestItem>
    {
        private static readonly string _script = $@"
SELECT
    id AS {nameof(TestItem.Id)}
   ,value AS {nameof(TestItem.Value)}
FROM test_items
WHERE id = @Id
;";

        public async Task<ApiResponse<TestItem>> Handle(SqlHattemSession connection, GetTestItemStructQuery query)
        {
            var sqlConnection = await connection.GetConnectionAsync();
            var value = await sqlConnection.QuerySingleAsync<TestItem>(_script, query);

            return ApiResponse.Ok(value);
        }
    }
}
