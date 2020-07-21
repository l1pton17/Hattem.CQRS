using Hattem.CQRS.DependencyInjection;

namespace Hattem.CQRS.Sql.DependencyInjection
{
    public static class SqlHattemCQRSServiceCollectionExtensions
    {
        public static CQRSBuilder AddSql(this CQRSBuilder builder, IDbConnectionFactory dbConnectionFactory)
        {


            return builder;
        }
    }
}
