using Hattem.CQRS.Commands;
using Hattem.CQRS.Queries;

namespace Hattem.CQRS
{
    public interface IHattemSession
    {
        IQueryProcessor QueryProcessor { get; }

        ICommandProcessor CommandProcessor { get; }
    }
}