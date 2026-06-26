namespace LunaWash.BLL.DTOs
{
    public class CreateReviewDto
    {
        public string BookingId { get; set; } = string.Empty;
        public int OverallRating { get; set; }
        public int CleanlinessRating { get; set; }
        public int SpeedRating { get; set; }
        public int StaffRating { get; set; }
        public string? Comment { get; set; }
    }
}