using MerchShop.Models;
using MerchShop.Models.DataAccessLayer;
using MerchShop.Models.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MerchShop.Controllers;

[Authorize(Roles = "Admin")] // restrict access to admin users only
public class CustomerController : Controller
{
    private readonly MerchShopContext _context;

    public CustomerController(MerchShopContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var customers = _context.Customers.ToList();
        return View(customers);
    }

    // GET: Customer/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> SaveChanges([FromBody] Customer customer)
    {
        if (customer.FirstName.Trim() != "" && customer.LastName.Trim() != "" && customer.Email.Trim() != "")
        {
            try
            {
                if (customer.CustomerID == 0)
                {
                    _context.Customers.Add(customer);
                }
                else
                {
                    _context.Update(customer);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CustomerExists(customer.CustomerID))
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

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.CustomerID == id);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Retrieve the vendor from the database
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Remove the vendor from the database
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            // Return success message
            return Ok("Customer deleted successfully");
        }
        catch (Exception ex)
        {
            // Return error message
            return BadRequest(ex.Message);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
