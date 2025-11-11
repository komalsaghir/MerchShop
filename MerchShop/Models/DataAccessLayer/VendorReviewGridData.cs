using MerchShop.Models.DomainModels;
using System.Text.Json.Serialization;

namespace MerchShop.Models
{
    public class VendorReviewGridData : GridData
    {
        public VendorReviewGridData() => SortField = nameof(VendorReview.Vendor.Name);

        [JsonIgnore]
        public bool IsSortByTitle => SortField.EqualsNoCase(nameof(VendorReview.Vendor.Name));
    }
}
