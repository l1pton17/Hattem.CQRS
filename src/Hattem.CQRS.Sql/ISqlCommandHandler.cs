using Hattem.CQRS.Commands;

namespace Hattem.CQRS.Sql
{
    public interface ISqlCommandHandler<in TCommand> : ICommandHandler<SqlHattemSession, TCommand>
        where TCommand : ICommand
    {
    }

    public interface ISqlCommandHandler<in TCommand, TReturn> : ICommandHandler<SqlHattemSession, TCommand, TReturn>
        where TCommand : ICommand<TReturn>
    {
    }
}
