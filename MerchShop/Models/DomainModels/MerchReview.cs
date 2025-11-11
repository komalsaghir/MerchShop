namespace MerchShop.Models.DomainModels
{
    public class MerchReview
    {
        public int MerchID { get; set; }
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        public int ReviewScore { get; set; }
        public string ReviewText { get; set; }

        public Merch Merch { get; set; }
        public Customer Customer { get; set; }
    }
}
