using MerchShop.Models.DomainModels;
using System.Text.Json.Serialization;

namespace MerchShop.Models
{
    public class MerchGridData : GridData
    {
        public MerchGridData() => SortField = nameof(Merch.ItemName);

        [JsonIgnore]
        public bool IsSortByTitle => SortField.EqualsNoCase(nameof(Merch.ItemName));
    }
}
