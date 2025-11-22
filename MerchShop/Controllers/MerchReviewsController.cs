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
			// --- Get logged in user (exit early if missing) ---
			var email = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(email))
				return View();

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return View();

			var customer = await _context.Customers
				.FirstOrDefaultAsync(c => c.UserID == user.Id);

			if (customer == null)
				return View();


			// --- Build Query Options ---
			var options = new QueryOptions<MerchReview>
			{
				Includes = "Merch",
				Where = r => r.CustomerID == customer.CustomerID,
				PageNumber = values.PageNumber,
				PageSize = values.PageSize,
				OrderByDirection = values.SortDirection
			};

			// --- Sorting Logic ---
			options.OrderBy = values.SortField switch
			{
				"Title" => r => r.Merch.ItemName,
				"Type" => r => r.Merch.Type,
				_ => null
			};


			// --- Build ViewModel ---
			var vm = new MerchReviewListViewModel
			{
				MerchReviews = _merchReviewsData.List(options),
				CurrentRoute = values,
				TotalPages = values.GetTotalPages(_merchData.Count)
			};


			return View(vm);
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
			// --- Validate input ---
			if (review.MerchID == 0 || string.IsNullOrWhiteSpace(review.ReviewText))
				return BadRequest("Invalid data. Fill all fields");

			if (review.ReviewScore > 5)
				return Ok("Rating Score must be less than or equal to 5");


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

				// --- Insert or update review ---
				var existingReview = _merchReviewsData.Get()
					.FirstOrDefault(r => r.MerchID == review.MerchID
									  && r.CustomerID == customer.CustomerID);

				if (existingReview != null)
				{
					_merchReviewsData.Update(review);
				}
				else
				{
					review.CustomerID = customer.CustomerID;
					_merchReviewsData.Insert(review);
				}

				_merchReviewsData.Save();
				return Ok("Merch Review saved successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: /Inventory/Delete/5
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				// --- Resolve user ---
				var email = User.FindFirstValue(ClaimTypes.Email);
				if (email == null)
					return BadRequest("User not found");

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
					return BadRequest("User not found");

				// --- Resolve customer ---
				var customer = await _context.Customers
					.FirstOrDefaultAsync(c => c.UserID == user.Id);

				if (customer == null)
					return BadRequest("Customer not found");

				// --- Fetch review to delete ---
				var merchReview = _merchReviewsData.Get(new QueryOptions<MerchReview>
				{
					Where = m => m.MerchID == id && m.CustomerID == customer.CustomerID
				});

				if (merchReview == null)
					return NotFound("Review not found");

				// --- Delete ---
				_merchReviewsData.Delete(merchReview);
				_merchReviewsData.Save();

				return Ok("Review deleted successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


	}
}
