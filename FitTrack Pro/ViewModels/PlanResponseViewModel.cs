namespace FitTrack_Pro.ViewModels
{
    public class PlanResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationInDays { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
