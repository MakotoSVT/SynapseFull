using Synapse.Models;

namespace Synapse.Providers
{
    public interface IOrderProvider
    {
       Task<IEnumerable<Order>> FetchMedicalEquipmentOrders();
    }
}
