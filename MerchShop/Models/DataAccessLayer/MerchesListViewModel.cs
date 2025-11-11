using MerchShop.Models.DomainModels;

namespace MerchShop.Models
{ 

    public class MerchesListViewModel
    {
        public IEnumerable<Merch> Merches { get; set; } = new HashSet<Merch>(); 
        public MerchGridData CurrentRoute { get; set; } = new MerchGridData();
        public int TotalPages { get; set; } 
    }
}
