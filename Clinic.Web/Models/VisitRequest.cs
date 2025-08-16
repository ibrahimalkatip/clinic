using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Web.Models
{
	public class VisitRequest
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string PatientId { get; set; } = string.Empty;

		[Required]
		public string DoctorId { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string Type { get; set; } = "Checkup"; // Visit or Checkup

		[MaxLength(500)]
		public string? Notes { get; set; }

		public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

		[MaxLength(30)]
		public string Status { get; set; } = "Pending"; // Pending/Approved/Declined/Completed
	}
}


