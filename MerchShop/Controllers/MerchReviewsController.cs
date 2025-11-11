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
	public class MerchReviewsController : Controller
	{
		private readonly Repository<Merch> _merchData;
		private readonly Repository<Customer> _customerData;
		private readonly Repository<MerchReview> _merchReviewsData;
		private readonly UserManager<AppUser> _userManager;
		private readonly MerchShopContext _context;

		public MerchReviewsController(MerchShopContext context, UserManager<AppUser> userManager)
		{
			_customerData = new Repository<Customer>(context);
			_merchData = new Repository<Merch>(context);
			_merchReviewsData = new Repository<MerchReview>(context);
			_userManager = userManager;
			_context = context;
		}

		// GET: /Inventory
		[HttpGet]
		public async Task<IActionResult> Index(MerchReviewGridData values)
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			// Find the user based on the email address
			var user = await _userManager.FindByEmailAsync(userEmail);

			if (user != null)
			{
				var customer = _context.Customers.Where(x => x.UserID == user.Id).FirstOrDefault();

				if (customer != null)
				{
					var options = new QueryOptions<MerchReview>
					{
						Includes = "Merch",
						Where = a => a.CustomerID == customer.CustomerID,
						PageNumber = values.PageNumber,
						PageSize = values.PageSize,
						OrderByDirection = values.SortDirection
					};

					// Check if sorting is based on Title or Quantity etc
					if (values.IsSortByTitle)
						options.OrderBy = a => a.Merch.ItemName;
					else if (values.SortField == "Type")
						options.OrderBy = a => a.Merch.Type;

					// Update the ViewModel with sorted data
					var vm = new MerchReviewListViewModel
					{
						MerchReviews = _merchReviewsData.List(options),
						CurrentRoute = values,
						TotalPages = values.GetTotalPages(_merchData.Count)
					};

					return View(vm);
				}
				return View();
			}
			return View();
		}

		public IActionResult Details(int id)
		{
			var inventory = _merchData.Get(new QueryOptions<Merch>
			{
				Where = a => a.MerchID == id,
				Includes = "Vendor",
			}) ?? new Merch();

			return View(inventory);
		}

		[HttpGet]
		public IActionResult GetMerchesDetail()
		{
			var allMerches = _merchData.List(new QueryOptions<Merch>
			{
				Includes = "Inventory",
			}) ?? new List<Merch>();

			// Filter the merches to include only those that are available in inventory
			var availableMerches = allMerches.Where(merch => merch.Inventory.Count()>0);

			var finalAvailable= availableMerches.Select(x => new
				{
					Rating = x.Rating,
					ItemDescription = x.ItemDescription,
					MerchID = x.MerchID,
					ItemName = x.ItemName,
					Price = x.Inventory.First().SalePrice,
					Type = x.Type
				})
				.ToList();

			return Json(finalAvailable);
		}

		[HttpGet]
		public IActionResult GetMerches()
		{
			var merches = _merchData.Get().Select(x => new
			{
				MerchID = x.MerchID,
				ItemName = x.ItemName
			}).ToList();

			return Json(merches);
		}

		[HttpPost]
		public async Task<IActionResult> SaveChanges([FromBody] MerchReview review)
		{
			if (review.MerchID != 0 && review.ReviewText.Trim() != "")
			{
				if (review.ReviewScore > 5)
				{
					return Ok("Rating Score must be less than or equal to 5");
				}
				try
				{
					var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

					// Find the user based on the email address
					var user = await _userManager.FindByEmailAsync(userEmail);

					if (user != null)
					{
						// Find the customer based on the user ID
						var customer = _context.Customers.Where(x => x.UserID == user.Id).FirstOrDefault();

						if (customer != null)
						{
							var reviewExists = _merchReviewsData.Get().Where(x => x.MerchID == review.MerchID && x.CustomerID == review.CustomerID);
							if (reviewExists.Count() > 0)
							{
								_merchReviewsData.Update(review);
							}
							else
							{
								review.CustomerID = customer.CustomerID;
								_merchReviewsData.Insert(review);
							}
							_merchReviewsData.Save();
						}
					}
				}
				catch (Exception ex)
				{
					throw;
				}
				return Ok("Merch Review saved successfully"); // Return success message
			}
			else
			{
				return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
			}
		}

		// GET: /Inventory/Delete/5
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

				// Find the user based on the email address
				var user = await _userManager.FindByEmailAsync(userEmail);

				if (user != null)
				{
					// Find the customer based on the user ID
					var customer = _context.Customers.Where(x => x.UserID == user.Id).FirstOrDefault();

					if (customer != null)
					{

						// Retrieve the merch from the database
						var merch = _merchReviewsData.Get(new QueryOptions<MerchReview>
						{
							Where = a => a.MerchID == id && a.CustomerID == customer.CustomerID,
						});
						if (merch == null)
						{
							return NotFound();
						}

						// Remove the merch from the database
						_merchReviewsData.Delete(merch);
						_merchReviewsData.Save();

						// Return success message
						return Ok("Review deleted successfully");
					}
				}
				return Ok("Not found!");
			}
			catch (Exception ex)
			{
				// Return error message
				return BadRequest(ex.Message);
			}
		}

	}
}
