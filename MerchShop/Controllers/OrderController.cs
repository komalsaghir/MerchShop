using Microsoft.AspNetCore.Mvc;
using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MerchShop.Controllers
{
	[Authorize(Roles = "User")] // restrict access to normal users only
	public class OrderController : Controller
	{
		private readonly Repository<Order> _orderData;
		private readonly UserManager<AppUser> _userManager;
		private readonly MerchShopContext _context;

		public OrderController(MerchShopContext context, UserManager<AppUser> userManager)
		{
			_orderData = new Repository<Order>(context);
			_userManager = userManager;
			_context = context;
		}

		// GET: /Order
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> PlaceOrder([FromBody] List<OrderDetailModel> orderDetails)
		{
			// --- Validate request body ---
			if (!ModelState.IsValid)
				return BadRequest("Invalid data. Fill all fields");

			if (orderDetails == null || !orderDetails.Any())
				return BadRequest("Order cannot be empty");


			try
			{
				// --- Resolve user ---
				var email = User.FindFirstValue(ClaimTypes.Email);
				if (string.IsNullOrEmpty(email))
					return BadRequest("User not found");

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
					return BadRequest("User not found");


				// --- Resolve customer ---
				var customer = await _context.Customers
					.FirstOrDefaultAsync(c => c.UserID == user.Id);

				if (customer == null)
					return BadRequest("Customer not found");


				// --- Create order ---
				var order = new Order
				{
					CustomerID = customer.CustomerID,
					Date = DateTime.Now,
					Total = orderDetails.Sum(od => od.Price * od.Quantity)
				};

				_context.Orders.Add(order);
				await _context.SaveChangesAsync();


				// --- Create order lines ---
				foreach (var detail in orderDetails)
				{
					var inventoryItem = await _context.Inventory
						.FirstOrDefaultAsync(i => i.MerchID == detail.MerchID);

					if (inventoryItem == null)
						return BadRequest("Inventory not found. OUT OF STOCK!");

					var orderLine = new OrderLines
					{
						OrderID = order.OrderID,
						ItemID = detail.MerchID,
						Quantity = detail.Quantity,
						SubTotal = detail.Quantity * detail.Price,
						InventoryItemID = inventoryItem.ItemID
					};

					_context.OrderLines.Add(orderLine);
				}

				await _context.SaveChangesAsync();
				return Ok("Order placed successfully");
			}
			catch
			{
				return StatusCode(500, "An error occurred while processing the request");
			}
		}

	}
}
