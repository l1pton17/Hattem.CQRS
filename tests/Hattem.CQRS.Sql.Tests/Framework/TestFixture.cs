using System.Data;
using System.Threading.Tasks;
using Hattem.CQRS.DependencyInjection;
using Hattem.CQRS.Sql.DependencyInjection;
using Hattem.CQRS.Sql.Extensions;
using Hattem.CQRS.Sql.Tests.Framework.Commands.TestItems;
using Hattem.CQRS.Sql.Tests.Framework.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hattem.CQRS.Sql.Tests.Framework
{
    public sealed class TestFixture : IAsyncLifetime
    {
        public ISqlHattemSessionFactory CreateSessionFactory()
        {
            var services = new ServiceCollection();

            services.AddCQRS(
                builder => builder
                    .AddAssembly(typeof(TestsBase).Assembly)
                    .UseSql<LocalDbConnectionFactory>());

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<ISqlHattemSessionFactory>();
        }

        public ISqlHattemSession CreateSessionWithoutTransaction()
        {
            return CreateSessionFactory().Create();
        }

        public ISqlHattemSession CreateSession(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return CreateSessionFactory().Create(isolationLevel);
        }

        public Task InitializeAsync()
        {
            return CreateSessionFactory()
                .Execute(session => session.ExecuteCommand(AddTestItemTableCommand.Default));
        }

        public Task DisposeAsync()
        {
            return CreateSessionFactory()
                .Execute(session => session.ExecuteCommand(DropTestItemTableCommand.Default));
        }
    }
}
