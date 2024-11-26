using Synapse.Models;

namespace Synapse.Providers
{
    public interface IUpdateProvider
    {
        Task<int> SendAlertAndUpdateOrder(Order order);
    }
}
