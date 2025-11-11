namespace MerchShop.Models.DomainModels
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }

        public Customer Customer { get; set; }
        public ICollection<OrderLines> OrderLines { get; set; }
    }
}
