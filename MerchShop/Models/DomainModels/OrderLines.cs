namespace MerchShop.Models.DomainModels
{
    public class OrderLines
    {
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public int InventoryItemID { get; set; }

        public Order Order { get; set; }
        public Inventory Inventory { get; set; }
    }
}
