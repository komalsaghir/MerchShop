using MerchShop.Models.DomainModels;

namespace MerchShop.Models
{ 

    public class InventoryListViewModel
    {
        public IEnumerable<Inventory> Inventories { get; set; } = new HashSet<Inventory>(); 
        public InventoryGridData CurrentRoute { get; set; } = new InventoryGridData();
        public int TotalPages { get; set; } 
    }
}
