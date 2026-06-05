using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class VehicleResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!; // Maps to VehicleModel
        public string License { get; set; } = null!; // Maps to LicensePlate
        public string? Color { get; set; }
        public string? VehicleTypeId { get; set; }
        public string? VehicleTypeName { get; set; }
    }

    public class CreateVehicleRequestDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string License { get; set; } = null!;

        [StringLength(50)]
        public string? Color { get; set; }

        public string? VehicleTypeId { get; set; }
    }
}
