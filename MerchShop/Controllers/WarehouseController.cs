using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace MerchShop.Controllers
{
	public class WarehouseController : Controller
	{
		private Repository<Warehouse> _warehouseData { get; set; }

		public WarehouseController(MerchShopContext context) => _warehouseData = new Repository<Warehouse>(context);

		[HttpGet]
		public IActionResult Index(WarehouseGridData values)
		{
			var options = new QueryOptions<Warehouse>
			{
				PageNumber = values.PageNumber,
				PageSize = values.PageSize,
				OrderByDirection = values.SortDirection
			};

			// Check if sorting is based on Title or Type
			if (values.SortField == "WarehouseType")
				options.OrderBy = a => a.WarehouseType;
			else
				options.OrderBy = a => a.ShelfNumber;

			// Update the ViewModel with sorted data
			var vm = new WarehousesListViewModel
			{
				Warehouses = _warehouseData.List(options),
				CurrentRoute = values,
				TotalPages = values.GetTotalPages(_warehouseData.Count)
			};

			return View(vm);
		}

		public IActionResult Details(int id)
		{
			var warehouses = _warehouseData.Get(new QueryOptions<Warehouse>
			{
				Where = a => a.LocationID == id
			}) ?? new Warehouse();

			return View(warehouses);
		}

		[HttpGet]
		public IActionResult GetWarehouses()
		{
			var warehouses = _warehouseData.Get().Select(x => new 
			{
				LocationID = x.LocationID,
				WarehouseType = x.WarehouseType
			}).ToList();

			return Json(warehouses);
		}

		//[HttpPost]
		//public async Task<IActionResult> SaveChanges([FromBody] Merch merch)
		//{
		//	if (merch.ItemName.Trim() != "" && merch.Type.Trim() != "" && merch.ItemDescription.Trim() != ""
		//		&& merch.Rating != 0 && merch.VendorID != 0)
		//	{
		//		try
		//		{
		//			if (merch.MerchID == 0)
		//			{
		//				_merchData.Insert(merch);
		//			}
		//			else
		//			{
		//				_merchData.Update(merch);
		//			}
		//			_merchData.Save();
		//		}
		//		catch (DbUpdateConcurrencyException ex)
		//		{
		//			if (!MerchExists(merch.VendorID))
		//			{
		//				return NotFound();
		//			}
		//			else
		//			{
		//				throw;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			throw;
		//		}
		//		return Ok("Changes saved successfully"); // Return success message
		//	}
		//	else
		//	{
		//		return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
		//	}
		//}

		//private bool MerchExists(int id)
		//{
		//	var merch = _merchData.Get(id);
		//	if (merch != null)
		//		return true;
		//	else return false;
		//}

		//[HttpPost]
		//public async Task<IActionResult> Delete(int id)
		//{
		//	try
		//	{
		//		// Retrieve the merch from the database
		//		var merch = _merchData.Get(id);
		//		if (merch == null)
		//		{
		//			return NotFound();
		//		}

		//		// Remove the merch from the database
		//		_merchData.Delete(merch);
		//		_merchData.Save();

		//		// Return success message
		//		return Ok("Merch deleted successfully");
		//	}
		//	catch (Exception ex)
		//	{
		//		// Return error message
		//		return BadRequest(ex.Message);
		//	}
		//}
	}
}
