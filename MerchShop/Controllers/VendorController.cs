using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace MerchShop.Controllers;

[Authorize(Roles = "Admin")] // restrict access to admin users only
public class VendorController : Controller
{
	private readonly MerchShopContext _context;

	private readonly ILogger<VendorController> _logger;

	public VendorController(MerchShopContext context, ILogger<VendorController> logger)
	{
		_logger = logger;
		_context = context;
	}
	public IActionResult Index()
	{
		var vendors = _context.Vendors.ToList();
		return View(vendors);
	}

	// GET: Vendor/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var vendor = await _context.Vendors.FindAsync(id);
		if (vendor == null)
		{
			return NotFound();
		}
		return View(vendor);
	}

	[HttpPost]
	public async Task<IActionResult> SaveChanges([FromBody] Vendor vendor)
	{
		if (vendor.Name.Trim() != "" && vendor.WebURL.Trim() != "" && vendor.OverallRating != 0)
		{
			try
			{
				if (vendor.VendorID == 0)
				{
					_context.Vendors.Add(vendor);
				}
				else
				{
					_context.Update(vendor);
				}
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				if (!VendorExists(vendor.VendorID))
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

	private bool VendorExists(int id)
	{
		return _context.Vendors.Any(e => e.VendorID == id);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			// Retrieve the vendor from the database
			var vendor = await _context.Vendors.FindAsync(id);
			if (vendor == null)
			{
				return NotFound();
			}

			// Remove the vendor from the database
			_context.Vendors.Remove(vendor);
			await _context.SaveChangesAsync();

			// Return success message
			return Ok("Vendor deleted successfully");
		}
		catch (Exception ex)
		{
			// Return error message
			return BadRequest(ex.Message);
		}
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

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
