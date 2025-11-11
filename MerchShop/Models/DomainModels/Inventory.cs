namespace MerchShop.Models.DomainModels
{
    public class Inventory
    {
        public int ItemID { get; set; }
        public int MerchID { get; set; }
        public int LocationID { get; set; }
        public int Quantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }

        public Merch Merch { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
