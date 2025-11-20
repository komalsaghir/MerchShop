using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MerchShop.Controllers
{
	public class HomeController : Controller
	{
		private readonly Repository<Inventory> _inventoryData;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, MerchShopContext context)
		{
			_logger = logger;
			_inventoryData = new Repository<Inventory>(context);
		}

		public IActionResult Index()
		{
			var threshold = 10;
			var lowStockCount = _inventoryData.List(new QueryOptions<Inventory>
			{
				Where = i => i.Quantity < threshold
			}).Count();
			ViewBag.LowStockCount = lowStockCount;

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
