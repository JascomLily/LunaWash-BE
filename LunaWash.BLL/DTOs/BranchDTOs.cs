using System;

namespace LunaWash.BLL.DTOs
{
    public class BranchResponseDto
    {
        public string Id { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BranchCreateDto
    {
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class BranchUpdateDto
    {
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class BranchEquipmentDto
    {
        public string Id { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
    }

    public class BranchSlotDto
    {
        public string Id { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string SlotNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
