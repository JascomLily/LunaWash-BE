using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class CreateBookingRequestDTO
    {
        [Required]
        public string BranchId { get; set; } = null!;

        [Required]
        public string WashSlotId { get; set; } = null!;

        [Required]
        public string ServicePackageId { get; set; } = null!; // E.g., 'SRV-WSH-01'

        [Required]
        public string VehicleId { get; set; } = null!;

        [Required]
        public string TimeSlotId { get; set; } = null!; // E.g., 'T-0900'

        public bool IncludeInteriorClean { get; set; }

        public string PaymentMethod { get; set; } = "tien-mat"; // tien-mat or vnpay
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
