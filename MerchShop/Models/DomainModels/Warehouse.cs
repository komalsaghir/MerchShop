namespace MerchShop.Models.DomainModels
{
    public class Warehouse
    {
        public int LocationID { get; set; }
        public string WarehouseType { get; set; }
        public string ShelfNumber { get; set; }

        public ICollection<Inventory> Inventory { get; set; }
    }
}
