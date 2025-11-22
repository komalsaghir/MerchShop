using Microsoft.AspNetCore.Mvc;
using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MerchShop.Controllers
{
	public class VendorReviewsController : Controller
	{
		private readonly Repository<Vendor> _vendorData;
		private readonly Repository<Customer> _customerData;
		private readonly Repository<VendorReview> _vendorReviewsData;
		private readonly UserManager<AppUser> _userManager;
		private readonly MerchShopContext _context;

		public VendorReviewsController(MerchShopContext context, UserManager<AppUser> userManager)
		{
			_customerData = new Repository<Customer>(context);
			_vendorData = new Repository<Vendor>(context);
			_vendorReviewsData = new Repository<VendorReview>(context);
			_userManager = userManager;
			_context = context;
		}

		// GET: /Inventory
		[HttpGet]
		public async Task<IActionResult> Index(VendorReviewGridData values)
		{
			// --- Get user email ---
			var email = User.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(email))
				return View();

			// --- Resolve user ---
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return View();

			// --- Resolve customer ---
			var customer = await _context.Customers
				.FirstOrDefaultAsync(c => c.UserID == user.Id);

			if (customer == null)
				return View();


			// --- Build query options ---
			var options = new QueryOptions<VendorReview>
			{
				Includes = "Vendor",
				Where = r => r.CustomerID == customer.CustomerID,
				PageNumber = values.PageNumber,
				PageSize = values.PageSize,
				OrderByDirection = values.SortDirection,
				OrderBy = values.SortField switch
				{
					"Title" or null when values.IsSortByTitle => r => r.Vendor.Name,
					"ReviewText" => r => r.ReviewText,
					"ReviewScore" => r => r.ReviewScore,
					_ => null
				}
			};


			// --- Build view model ---
			var vm = new VendorReviewListViewModel
			{
				VendorReviews = _vendorReviewsData.List(options),
				CurrentRoute = values,
				TotalPages = values.GetTotalPages(_vendorData.Count)
			};

			return View(vm);
		}

		public IActionResult Details(int id)
		{
			var vendor = _vendorData.Get(new QueryOptions<Vendor>
			{
				Where = a => a.VendorID == id
			}) ?? new Vendor();

			return View(vendor);
		}

		[HttpGet]
		public IActionResult GetVendors()
		{
			var vendors = _context.Vendors.Select(v => new
			{
				VendorID = v.VendorID,
				Name = v.Name
			}).ToList();

			return Json(vendors);
		}

		[HttpPost]
		public async Task<IActionResult> SaveChanges([FromBody] VendorReview review)
		{
			// --- Input validation ---
			if (review.VendorID == 0 || string.IsNullOrWhiteSpace(review.ReviewText))
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


				// --- Check if review exists ---
				var existingReview = _vendorReviewsData.Get()
					.FirstOrDefault(r => r.VendorID == review.VendorID
									  && r.CustomerID == customer.CustomerID);

				// --- Update or insert ---
				if (existingReview != null)
				{
					_vendorReviewsData.Update(review);
				}
				else
				{
					review.CustomerID = customer.CustomerID;
					_vendorReviewsData.Insert(review);
				}

				_vendorReviewsData.Save();
				return Ok("Vendor Review saved successfully");
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
				if (string.IsNullOrEmpty(email))
					return Ok("Not found!");

				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
					return Ok("Not found!");

				// --- Resolve customer ---
				var customer = await _context.Customers
					.FirstOrDefaultAsync(c => c.UserID == user.Id);

				if (customer == null)
					return Ok("Not found!");

				// --- Fetch review ---
				var review = _vendorReviewsData.Get(new QueryOptions<VendorReview>
				{
					Where = r => r.VendorID == id && r.CustomerID == customer.CustomerID
				});

				if (review == null)
					return NotFound();

				// --- Delete review ---
				_vendorReviewsData.Delete(review);
				_vendorReviewsData.Save();

				return Ok("Review deleted successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
