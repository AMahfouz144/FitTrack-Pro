using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitTrack_Pro.ViewModels
{
    // ════════════════════════════════════════════════════════════════
    //  INDEX  –  paged list
    // ════════════════════════════════════════════════════════════════
    public class GymClassIndexViewModel
    {
        public IEnumerable<GymClassRowViewModel> GymClasses { get; set; } = [];
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public string? SearchQuery { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    // ────────────────────────────────────────────────────────────────
    //  Single row in the gym classes table
    // ────────────────────────────────────────────────────────────────
    public class GymClassRowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public int DurationInMinutes { get; set; }
        public int MaxCapacity { get; set; }
        public int AttendeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ════════════════════════════════════════════════════════════════
    //  DETAILS
    // ════════════════════════════════════════════════════════════════
    public class GymClassDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public DateTime ScheduleTime { get; set; }
        public int DurationInMinutes { get; set; }
        public int MaxCapacity { get; set; }
        public int AttendeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ════════════════════════════════════════════════════════════════
    //  CREATE / EDIT FORM
    // ════════════════════════════════════════════════════════════════
    public class GymClassFormViewModel
    {
        public int Id { get; set; }   // 0 = create

        [Required(ErrorMessage = "Class name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Class Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a trainer")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Schedule date & time is required")]
        [Display(Name = "Schedule Date & Time")]
        public DateTime ScheduleTime { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Duration is required")]
        [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int DurationInMinutes { get; set; } = 60;

        [Required(ErrorMessage = "Max capacity is required")]
        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500")]
        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; } = 20;

        // Populated by the service for the trainer dropdown
        public IEnumerable<SelectListItem> TrainerOptions { get; set; } = [];
    }
}
