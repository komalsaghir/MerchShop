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
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			// Find the user based on the email address
			var user = await _userManager.FindByEmailAsync(userEmail);

			if (user != null)
			{
				var customer = _context.Customers.Where(x => x.UserID == user.Id).FirstOrDefault();

				if (customer != null)
				{
					var options = new QueryOptions<VendorReview>
					{
						Includes = "Vendor",
						Where = a => a.CustomerID == customer.CustomerID,
						PageNumber = values.PageNumber,
						PageSize = values.PageSize,
						OrderByDirection = values.SortDirection
					};

					// Check if sorting is based on Title or Quantity etc
					if (values.IsSortByTitle)
						options.OrderBy = a => a.Vendor.Name;
					else if (values.SortField == "ReviewText")
						options.OrderBy = a => a.ReviewText;
					else if (values.SortField == "ReviewScore")
						options.OrderBy = a => a.ReviewScore;

					// Update the ViewModel with sorted data
					var vm = new VendorReviewListViewModel
					{
						VendorReviews = _vendorReviewsData.List(options),
						CurrentRoute = values,
						TotalPages = values.GetTotalPages(_vendorData.Count)
					};

					return View(vm);
				}
				return View();
			}
			return View();
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
			if (review.VendorID != 0 && review.ReviewText.Trim() != "")
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
							var reviewExists = _vendorReviewsData.Get().Where(x => x.VendorID == review.VendorID && x.CustomerID == review.CustomerID);
							if (reviewExists.Count() > 0)
							{
								_vendorReviewsData.Update(review);
							}
							else
							{
								review.CustomerID = customer.CustomerID;
								_vendorReviewsData.Insert(review);
							}
							_vendorReviewsData.Save();
						}
					}
				}
				catch (Exception ex)
				{
					throw;
				}
				return Ok("Vendor Review saved successfully"); // Return success message
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
						var merch = _vendorReviewsData.Get(new QueryOptions<VendorReview>
						{
							Where = a => a.VendorID == id && a.CustomerID == customer.CustomerID,
						});
						if (merch == null)
						{
							return NotFound();
						}

						// Remove the merch from the database
						_vendorReviewsData.Delete(merch);
						_vendorReviewsData.Save();

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
