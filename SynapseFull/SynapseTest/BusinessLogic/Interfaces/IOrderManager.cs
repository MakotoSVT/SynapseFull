using Synapse.Models;

namespace Synapse.BusinessLogic
{
    public interface IOrderManager
    {
        Order ProcessOrder(Order order);
    }
}
