namespace MerchShop.Models.DomainModels
{
    public class VendorReview
    {
        public int VendorID { get; set; }
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        public int ReviewScore { get; set; }
        public string ReviewText { get; set; }

        public Vendor Vendor { get; set; }
        public Customer Customer { get; set; }
    }
}
