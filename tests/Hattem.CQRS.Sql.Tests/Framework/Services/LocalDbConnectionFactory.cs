using System;
using System.Data.Common;
using Npgsql;

namespace Hattem.CQRS.Sql.Tests.Framework.Services
{
    public sealed class LocalDbConnectionFactory : IDbConnectionFactory
    {
        public DbConnection Create()
        {
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";

            return new NpgsqlConnection($"Host={host};Port=5432;Database=test;Username=test;Password=test;");
        }
    }
}
