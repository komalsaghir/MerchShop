using MerchShop.Models.DomainModels;

namespace MerchShop.Models
{ 

    public class MerchReviewListViewModel
    {
        public IEnumerable<MerchReview> MerchReviews { get; set; } = new HashSet<MerchReview>(); 
        public MerchReviewGridData CurrentRoute { get; set; } = new MerchReviewGridData();
        public int TotalPages { get; set; } 
    }
}
