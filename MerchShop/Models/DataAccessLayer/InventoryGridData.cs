using MerchShop.Models.DomainModels;
using System.Text.Json.Serialization;

namespace MerchShop.Models
{
    public class InventoryGridData : GridData
    {
        public InventoryGridData() => SortField = nameof(Inventory.Merch.ItemName);

        [JsonIgnore]
        public bool IsSortByTitle => SortField.EqualsNoCase(nameof(Inventory.Merch.ItemName));
    }
}
