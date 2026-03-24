using System.ComponentModel.DataAnnotations;

namespace FitTrack_Pro.ViewModels
{
    public class PlanRequestViewModel
    {
        [Required]
        [Length(1, 20)]
        public string Name { get; set; } = string.Empty;
        [Range(7,1000)]
        public int DurationInDays { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
