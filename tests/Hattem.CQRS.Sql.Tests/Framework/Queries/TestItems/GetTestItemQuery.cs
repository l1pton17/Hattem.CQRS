using System.Threading.Tasks;
using Dapper;
using Hattem.Api;
using Hattem.CQRS.Queries;
using Hattem.CQRS.Sql.Tests.Framework.Models;

namespace Hattem.CQRS.Sql.Tests.Framework.Queries.TestItems
{
    public sealed class GetTestItemQuery : IQuery<TestItem>
    {
        public int Id { get; }

        public GetTestItemQuery(int id)
        {
            Id = id;
        }
    }

    public sealed class GetTestItemQueryHandler : ISqlQueryHandler<GetTestItemQuery, TestItem>
    {
        private static readonly string _script = $@"
SELECT
    id AS {nameof(TestItem.Id)}
   ,value AS {nameof(TestItem.Value)}
FROM test_items
WHERE id = @Id
;";

        public async Task<ApiResponse<TestItem>> Handle(SqlHattemSession connection, GetTestItemQuery query)
        {
            var sqlConnection = await connection.GetConnectionAsync();
            var value = await sqlConnection.QuerySingleAsync<TestItem>(_script, query);

            return ApiResponse.Ok(value);
        }
    }
}
