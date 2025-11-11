using MerchShop.Models.DomainModels;

namespace MerchShop.Models
{ 

    public class VendorReviewListViewModel
    {
        public IEnumerable<VendorReview> VendorReviews { get; set; } = new HashSet<VendorReview>(); 
        public VendorReviewGridData CurrentRoute { get; set; } = new VendorReviewGridData();
        public int TotalPages { get; set; } 
    }
}
