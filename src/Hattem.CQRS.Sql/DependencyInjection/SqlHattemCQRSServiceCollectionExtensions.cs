using Hattem.CQRS.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hattem.CQRS.Sql.DependencyInjection
{
    public static class SqlHattemCQRSServiceCollectionExtensions
    {
        public static CQRSBuilder UseSql<TDbConnectionFactory>(this CQRSBuilder builder)
            where TDbConnectionFactory : class, IDbConnectionFactory
        {
            builder.Services.TryAddSingleton<IDbConnectionFactory, TDbConnectionFactory>();

            return builder
                .UseConnection<ISqlHattemSessionFactory, SqlHattemSessionFactory, SqlHattemSession>();
        }
    }
}
