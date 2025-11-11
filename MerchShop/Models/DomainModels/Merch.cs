namespace MerchShop.Models.DomainModels
{
    public class Merch
    {
        public int MerchID { get; set; }
        public int VendorID { get; set; }
        public string Type { get; set; }
        public string ItemName { get; set; }
        public float Rating { get; set; }
        public string ItemDescription { get; set; }

        public Vendor Vendor { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public ICollection<MerchReview> MerchReviews { get; set; }
    }
}
