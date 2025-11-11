using MerchShop.Models.DomainModels;
using System.Text.Json.Serialization;

namespace MerchShop.Models
{
    public class WarehouseGridData : GridData
    {
        public WarehouseGridData() => SortField = nameof(Warehouse.WarehouseType);

        [JsonIgnore]
        public bool IsSortByTitle => SortField.EqualsNoCase(nameof(Warehouse.WarehouseType));
    }
}
