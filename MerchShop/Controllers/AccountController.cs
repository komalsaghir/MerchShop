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

[ApiController]
[Route("[controller]/[action]")]
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
		if (!ModelState.IsValid)
			return View(model);

		var signedUser = await _userManager.FindByEmailAsync(model.Email);
		if (signedUser == null)
		{
			ModelState.AddModelError("Email", "Invalid login attempt.");
			return View(model);
		}

		var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
		if (result.Succeeded)
		{
			// Add email claim if not already present
			var claims = await _userManager.GetClaimsAsync(signedUser);
			if (!claims.Any(c => c.Type == ClaimTypes.Email && c.Value == model.Email))
			{
				await _userManager.AddClaimAsync(signedUser, new Claim(ClaimTypes.Email, model.Email));
			}
			return RedirectToAction("Index", "Home");
		}

		ModelState.AddModelError("Email", "Invalid login attempt.");
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
		if (!ModelState.IsValid)
			return View(model);

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
			UserName = model.Email,
			NormalizedUserName = model.Email.ToUpper(),
			IsActive = true,
			PhoneNumber = model.PhoneNumber,
			BirthDate = model.BirthDate
		};

		var result = await _userManager.CreateAsync(newUser, model.Password);

		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(newUser, "User");

			var customer = new Customer
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				UserID = newUser.Id
			};

			_context.Customers.Add(customer);
			await _context.SaveChangesAsync();

			// Redirect to login after successful registration
			return RedirectToAction("Login", "Account");
		}

		// If creation fails, add model errors
		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(Login), "Account");
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
