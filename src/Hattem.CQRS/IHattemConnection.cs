using System.Threading.Tasks;
using Hattem.Api;
using Hattem.CQRS.Notifications;

namespace Hattem.CQRS
{
    public interface IHattemConnection
    {
        Task<ApiResponse<Unit>> PublishNotification<T>(T notification)
            where T : INotification;
    }
}