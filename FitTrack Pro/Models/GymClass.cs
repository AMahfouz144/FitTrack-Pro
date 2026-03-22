using System.ComponentModel.DataAnnotations;

namespace FitTrack_Pro.Models
{
	public class GymClass : BaseEntity
	{
		[Required, StringLength(100)]
		public string Name { get; set; } = string.Empty;

		public int TrainerId { get; set; }
		public Trainer? Trainer { get; set; }

		public DateTime ScheduleTime { get; set; }
		public int DurationInMinutes { get; set; }
		public int MaxCapacity { get; set; }

		public  ICollection<ClassAttendance> Attendees { get; set; }
	}
}
