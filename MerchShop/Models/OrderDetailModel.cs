using System.ComponentModel.DataAnnotations;

namespace MerchShop.Models
{
	public class OrderDetailModel
	{
		[Required]
		public int MerchID { get; set; }
		[Required]
		public int Quantity { get; set; }
		[Required]
		public int Price { get; set; }
	}
}
