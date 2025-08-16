using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Web.Models
{
	public class ApplicationUser : IdentityUser
	{
		[MaxLength(100)]
		public string? FullName { get; set; }

		[MaxLength(200)]
		public string? Address { get; set; }

		[MaxLength(50)]
		public string? Status { get; set; }
	}
}


