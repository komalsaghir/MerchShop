using MerchShop.Models.DomainModels;
using System.Text.Json.Serialization;

namespace MerchShop.Models
{
    public class MerchReviewGridData : GridData
    {
        public MerchReviewGridData() => SortField = nameof(MerchReview.Merch.ItemName);

        [JsonIgnore]
        public bool IsSortByTitle => SortField.EqualsNoCase(nameof(MerchReview.Merch.ItemName));
    }
}
