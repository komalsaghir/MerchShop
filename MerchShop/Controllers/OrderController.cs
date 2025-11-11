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
			try
			{
				if (ModelState.IsValid)
				{
					var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

					// Find the user based on the email address
					var user = await _userManager.FindByEmailAsync(userEmail);

					if (user != null)
					{
						// Get the user ID
						var userId = user.Id;

						// Find the customer based on the user ID
						var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserID == userId);

						if (customer != null)
						{
							// Create the order entity
							var order = new Order
							{
								CustomerID = customer.CustomerID,
								Date = DateTime.Now,
								Total = orderDetails.Sum(od => od.Quantity * od.Price) // Calculate the total based on order details
							};

							// Add the order to the database context
							_context.Orders.Add(order);
							await _context.SaveChangesAsync();

							// Create order line entities and add them to the database context
							foreach (var orderDetail in orderDetails)
							{
								// Query the database to retrieve the InventoryItemID based on the MerchID
								var inventoryItem = await _context.Inventory.FirstOrDefaultAsync(i => i.MerchID == orderDetail.MerchID);

								if (inventoryItem != null)
								{
									var orderLine = new OrderLines
									{
										OrderID = order.OrderID,
										ItemID = orderDetail.MerchID,
										Quantity = orderDetail.Quantity,
										SubTotal = orderDetail.Quantity * orderDetail.Price,
										InventoryItemID = inventoryItem.ItemID,
									};

									_context.OrderLines.Add(orderLine);
								}
								else
								{
									// Handle the case where InventoryItemID is not found for the given MerchID
									// You can log an error or take appropriate action
									return BadRequest("Inventory not found. OUT OF STOCK!");
								}
							}


							await _context.SaveChangesAsync();

							return Ok("Order placed successfully");
						}
						else
						{
							return BadRequest("Customer not found");
						}
					}
					else
					{
						return BadRequest("User not found");
					}
				}
				else
				{
					return BadRequest("Invalid data. Fill all fields");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while processing the request");
			}
		}
	}
}
