namespace Synapse.Models
{
    public class Item
    {
        // I am following the OrderId convention without knowing if Items follow the same pattern because all things need identifiers of some sort.
        public string ItemId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int DeliveryNotification { get; set; }
    }
}
