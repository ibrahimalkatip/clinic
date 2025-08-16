using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Clinic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View(new RegisterViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FullName = model.FullName,
				Address = model.Address
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View(model);
			}

			// Add to Patient role by default
			await _userManager.AddToRoleAsync(user, "Patient");
			await _signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction("Dashboard", "Patient");
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
				{
					return LocalRedirect(model.ReturnUrl);
				}
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}
	}

	public sealed class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[Compare(nameof(Password))]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string FullName { get; set; } = string.Empty;

		[StringLength(200)]
		public string? Address { get; set; }
	}

	public sealed class LoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		public bool RememberMe { get; set; }

		public string? ReturnUrl { get; set; }
	}
}


