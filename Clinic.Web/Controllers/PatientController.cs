using Clinic.Web.Data;
using Clinic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Clinic.Web.Controllers
{
	[Authorize(Roles = "Patient")]
	public class PatientController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<ApplicationUser> _userManager;

		public PatientController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<IActionResult> Dashboard()
		{
			var userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId!);
			var myRequests = await _dbContext.VisitRequests
				.Where(v => v.PatientId == userId)
				.OrderByDescending(v => v.RequestedAt)
				.ToListAsync();
			ViewBag.User = user;
			var counts = myRequests
				.GroupBy(r => r.Status ?? "Unknown")
				.Select(g => new { status = g.Key, count = g.Count() })
				.OrderByDescending(x => x.count)
				.ToList();
			ViewBag.StatusCountsJson = JsonSerializer.Serialize(counts);
			return View(myRequests);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateProfile(string fullName, string? address, string? status)
		{
			var userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId!);
			if (user == null)
			{
				return NotFound();
			}
			user.FullName = fullName;
			user.Address = address;
			user.Status = status;
			await _userManager.UpdateAsync(user);
			return RedirectToAction(nameof(Dashboard));
		}
	}
}


