using System.ComponentModel.DataAnnotations;

namespace FitTrack_Pro.ViewModels
{
	public class MemberVisitFormViewModel
	{
		public int ClassAttendanceId { get; set; }

		public int MemberId { get; set; }
		public int GymClassId { get; set; }

		[Required(ErrorMessage = "Please enter height")]
		[Display(Name = "Height (m)")]
		[Range(0.5, 3.0, ErrorMessage = "Invalid height")]
		public decimal Height { get; set; }

		[Required(ErrorMessage = "Please enter weight")]
		[Display(Name = "Weight (kg)")]
		[Range(20, 300, ErrorMessage = "Invalid weight")]
		public decimal Weight { get; set; }

		[Display(Name = "Trainer Notes")]
		[DataType(DataType.MultilineText)]
		public string? Notes { get; set; }
	}
}
