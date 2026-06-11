using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class CreateBookingRequestDTO
    {
        [Required]
        public string BranchId { get; set; } = null!;
        public string? WashSlotId { get; set; }
        [Required]
        public string VehicleTypeId { get; set; } = null!;
        public int Duration { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
        public string? LicensePlate { get; set; }
        [Required]
        public DateTime ScheduledStartTime { get; set; }
        public string? Notes { get; set; }
        public List<string> ServicePriceIds { get; set; } = new List<string>();
    }

    public class OccupiedSlotDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class BookingResponseDTO
    {
        public string Id { get; set; } = null!;
        public string PackageName { get; set; } = null!;
        public string Services { get; set; } = null!;
        public string VehicleInfo { get; set; } = null!;
        public string Extras { get; set; } = null!;
        public string BranchInfo { get; set; } = null!;
        public string SlotName { get; set; } = null!;
        public string TimeRange { get; set; } = null!;
        public string TotalPrice { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime BookingDate { get; set; }
    }
}
