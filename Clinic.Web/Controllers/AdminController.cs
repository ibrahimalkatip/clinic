using Clinic.Web.Data;
using Clinic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<ApplicationUser> _userManager;

		public AdminController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<IActionResult> Patients()
		{
			var patients = await _userManager.GetUsersInRoleAsync("Patient");
			return View(patients);
		}

		public async Task<IActionResult> VisitRequests()
		{
			var requests = await _dbContext.VisitRequests
				.OrderByDescending(v => v.RequestedAt)
				.ToListAsync();
			return View(requests);
		}

		[HttpPost]
		public async Task<IActionResult> SendVisitRequest(string patientId, string type, string? notes)
		{
			var doctorId = _userManager.GetUserId(User);
			if (doctorId == null)
			{
				return Unauthorized();
			}

			var request = new VisitRequest
			{
				PatientId = patientId,
				DoctorId = doctorId,
				Type = string.IsNullOrWhiteSpace(type) ? "Checkup" : type,
				Notes = notes,
				Status = "Pending"
			};

			_dbContext.VisitRequests.Add(request);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(VisitRequests));
		}

		[HttpPost]
		public async Task<IActionResult> UpdateRequestStatus(int id, string status)
		{
			var request = await _dbContext.VisitRequests.FindAsync(id);
			if (request == null)
			{
				return NotFound();
			}
			request.Status = status;
			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(VisitRequests));
		}
	}
}


