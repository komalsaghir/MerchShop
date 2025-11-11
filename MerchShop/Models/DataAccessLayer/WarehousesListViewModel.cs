using MerchShop.Models.DomainModels;

namespace MerchShop.Models
{ 

    public class WarehousesListViewModel
    {
        public IEnumerable<Warehouse> Warehouses { get; set; } = new HashSet<Warehouse>(); 
        public WarehouseGridData CurrentRoute { get; set; } = new WarehouseGridData();
        public int TotalPages { get; set; } 
    }
}
