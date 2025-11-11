namespace MerchShop.Models.DomainModels
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserID { get; set; }

        public ICollection<VendorReview> VendorReviews { get; set; }
        public ICollection<MerchReview> MerchReviews { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
