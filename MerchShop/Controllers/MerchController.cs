using Microsoft.AspNetCore.Mvc;
using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MerchShop.Controllers
{
	[Authorize(Roles = "Admin")] // restrict access to admin users only
	public class MerchController : Controller
	{
		private Repository<Merch> _merchData { get; set; }

		public MerchController(MerchShopContext context) => _merchData = new Repository<Merch>(context);

		[HttpGet]
		public IActionResult Index(MerchGridData values)
		{
			var options = new QueryOptions<Merch>
			{
				Includes = "Vendor, Inventory",
				PageNumber = values.PageNumber,
				PageSize = values.PageSize,
				OrderByDirection = values.SortDirection
			};

			// Check if sorting is based on Title or Type
			if (values.IsSortByTitle)
				options.OrderBy = a => a.ItemName;
			else if (values.SortField == "Type")
				options.OrderBy = a => a.Type;
			else
				options.OrderBy = a => a.Vendor.Name;

			// Update the ViewModel with sorted data
			var vm = new MerchesListViewModel
			{
				Merches = _merchData.List(options),
				CurrentRoute = values,
				TotalPages = values.GetTotalPages(_merchData.Count)
			};

			return View(vm);
		}

		public IActionResult Details(int id)
		{
			var merches = _merchData.Get(new QueryOptions<Merch>
			{
				Where = a => a.MerchID == id,
				Includes = "Vendor, Inventory",
			}) ?? new Merch();

			return View(merches);
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
		public async Task<IActionResult> SaveChanges([FromBody] Merch merch)
		{
			if (merch.ItemName.Trim() != "" && merch.Type.Trim() != "" && merch.ItemDescription.Trim() != ""
				&& merch.Rating != 0 && merch.VendorID != 0)
			{
				try
				{
					if (merch.MerchID == 0)
					{
						_merchData.Insert(merch);
					}
					else
					{
						_merchData.Update(merch);
					}
					_merchData.Save();
				}
				catch (DbUpdateConcurrencyException ex)
				{
					if (!MerchExists(merch.VendorID))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				catch (Exception ex)
				{
					throw;
				}
				return Ok("Changes saved successfully"); // Return success message
			}
			else
			{
				return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
			}
		}

		private bool MerchExists(int id)
		{
			var merch = _merchData.Get(id);
			if (merch != null)
				return true;
			else return false;
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				// Retrieve the merch from the database
				var merch = _merchData.Get(id);
				if (merch == null)
				{
					return NotFound();
				}

				// Remove the merch from the database
				_merchData.Delete(merch);
				_merchData.Save();

				// Return success message
				return Ok("Merch deleted successfully");
			}
			catch (Exception ex)
			{
				// Return error message
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IActionResult LogMerchAction(int merchId, string actionType)
		{
			// Hardcoded logging logic with poor naming and mixed concerns
			string logMessage = "Action performed on merch: " + merchId + " with type: " + actionType;
			System.IO.File.AppendAllText("C:\\temp\\merchlog.txt", logMessage + "\n");

			if (actionType == "delete")
			{
				var merch = _merchData.Get(merchId);
				if (merch != null)
				{
					_merchData.Delete(merch);
					_merchData.Save();
					return Ok("Deleted and logged");
				}
				else
				{
					return Ok("Not found but logged");
				}
			}
			else if (actionType == "update")
			{
				var merch = _merchData.Get(merchId);
				if (merch != null)
				{
					merch.ItemName = merch.ItemName + "_updated";
					_merchData.Update(merch);
					_merchData.Save();
					return Ok("Updated and logged");
				}
				else
				{
					return Ok("Not found but logged");
				}
			}
			else
			{
				return Ok("Logged only");
			}
		}

	}
}
