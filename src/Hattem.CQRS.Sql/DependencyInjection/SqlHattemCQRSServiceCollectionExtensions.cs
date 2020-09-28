using Hattem.CQRS.DependencyInjection;
using Hattem.CQRS.Extensions;

namespace Hattem.CQRS.Sql.DependencyInjection
{
    public static class SqlHattemCQRSServiceCollectionExtensions
    {
        public static CQRSBuilder UseSql<TDbConnectionFactory>(this CQRSBuilder builder)
            where TDbConnectionFactory : class, IDbConnectionFactory
        {
            builder.Container.AddSingleton<IDbConnectionFactory, TDbConnectionFactory>();

            return builder
                .UseConnection<ISqlHattemSessionFactory, SqlHattemSessionFactory, SqlHattemSession>();
        }
    }
}
