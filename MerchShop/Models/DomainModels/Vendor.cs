namespace MerchShop.Models.DomainModels
{
public class Vendor
    {
        public int VendorID { get; set; }
        public string Name { get; set; }
        public string WebURL { get; set; }
        public float OverallRating { get; set; }

        public ICollection<Merch> Merch { get; set; }
        public ICollection<VendorReview> VendorReviews { get; set; }
    }
}
