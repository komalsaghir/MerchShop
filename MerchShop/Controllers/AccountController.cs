using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MerchShop.Controllers;
using MerchShop.Models;
using MerchShop.Models.DomainModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using MerchShop.Models.DataAccessLayer;
namespace MerchShop.Controllers;

public class AccountController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly MerchShopContext _context;

	public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, MerchShopContext context)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_context = context;
	}

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			AppUser signedUser = await _userManager.FindByEmailAsync(model.Email);
			var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				await _userManager.AddClaimAsync(signedUser, new Claim(ClaimTypes.Email, model.Email));
				return RedirectToAction("Index", "Home");
			}
			ModelState.AddModelError("Email", "Invalid login attempt.");
			return View(model);
		}
		return View(model);
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			// Check if the email already exists
			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("Email", "Email address is already registered. Login!");
				return View(model);
			}

			// Create a new user
			var newUser = new AppUser
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				NormalizedEmail = model.Email.ToUpper(),
				UserName = model.Email, // Setting the username as the email address
				NormalizedUserName = model.Email.ToUpper(),
				IsActive = true,
				PhoneNumber = model.PhoneNumber,
				BirthDate = model.BirthDate
			};

			// Attempt to create the user
			var result = await _userManager.CreateAsync(newUser, model.Password);

			if (result.Succeeded)
			{
				var userrole = await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(model.Email), "User");

				Customer customer = new Customer()
				{
					FirstName = "Robert",
					LastName = "Brown",
					Email = model.Email,
					UserID = newUser.Id
				};

				_context.Customers.Add(customer);
				_context.SaveChanges();
				
				// You may want to sign in the user after registration, 
				// or redirect them to a confirmation page and let them sign in manually
				return RedirectToAction("Login", "Account");
			}
			else
			{
				// If creation fails, add model errors
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View(model);
			}
		}
		// If ModelState is not valid, return the view with the model
		return View(model);
	}


	//[HttpPost]
	//[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(AccountController.Login), "Account");
	}

	private IActionResult RedirectToLocal(string returnUrl)
	{
		if (Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}
		return RedirectToAction(nameof(HomeController.Index), "Home");
	}
}
