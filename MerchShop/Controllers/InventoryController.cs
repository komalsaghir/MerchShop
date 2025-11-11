using Microsoft.AspNetCore.Mvc;
using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace MerchShop.Controllers
{
	public class InventoryController : Controller
	{
		private readonly Repository<Inventory> _inventoryData;
		private readonly Repository<Warehouse> _warehouseData;

		public InventoryController(MerchShopContext context)
		{
			_inventoryData = new Repository<Inventory>(context);
			_warehouseData = new Repository<Warehouse>(context);
		}

		// GET: /Inventory
		[HttpGet]
		public IActionResult Index(InventoryGridData values)
		{
			var options = new QueryOptions<Inventory>
			{
				Includes = "Merch, Warehouse",
				PageNumber = values.PageNumber,
				PageSize = values.PageSize,
				OrderByDirection = values.SortDirection
			};

			// Check if sorting is based on Title or Quantity etc
			if (values.IsSortByTitle)
				options.OrderBy = a => a.Merch.ItemName;
			else if (values.SortField == "Quantity")
				options.OrderBy = a => a.Quantity;
			else if (values.SortField == "SalePrice")
				options.OrderBy = a => a.SalePrice;
			else
				options.OrderBy = a => a.PurchasePrice;

			// Update the ViewModel with sorted data
			var vm = new InventoryListViewModel
			{
				Inventories = _inventoryData.List(options),
				CurrentRoute = values,
				TotalPages = values.GetTotalPages(_inventoryData.Count)
			};

			return View(vm);
		}

		public IActionResult Details(int id)
		{
			var inventory = _inventoryData.Get(new QueryOptions<Inventory>
			{
				Where = a => a.ItemID == id,
				Includes = "Merch, Warehouse",
			}) ?? new Inventory();

			return View(inventory);
		}

		[HttpPost]
		public async Task<IActionResult> SaveChanges([FromBody] Inventory inventory)
		{
			if (inventory.LocationID!=0 && inventory.MerchID!=0
				&& inventory.PurchasePrice != 0 && inventory.SalePrice != 0 )
			{
				try
				{
					if (inventory.ItemID == 0)
					{
						inventory.Warehouse = _warehouseData.Get(inventory.LocationID);
						_inventoryData.Insert(inventory);
					}
					else
					{
						_inventoryData.Update(inventory);
					}
					_inventoryData.Save();
				}
				catch (DbUpdateConcurrencyException ex)
				{
					if (!InventoryExists(inventory.ItemID))
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

		private bool InventoryExists(int id)
		{
			var inventory = _inventoryData.Get(id);
			if (inventory != null)
				return true;
			else return false;
		}

		// GET: /Inventory/Delete/5
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				// Retrieve the merch from the database
				var merch = _inventoryData.Get(id);
				if (merch == null)
				{
					return NotFound();
				}

				// Remove the merch from the database
				_inventoryData.Delete(merch);
				_inventoryData.Save();

				// Return success message
				return Ok("Inventory deleted successfully");
			}
			catch (Exception ex)
			{
				// Return error message
				return BadRequest(ex.Message);
			}
		}

	}
}
