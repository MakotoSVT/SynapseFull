namespace Synapse.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}
