using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class CreatePromotionDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int DiscountPercent { get; set; }

        public int? MaxUsage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class PromotionResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int DiscountPercent { get; set; }
        public int? MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = null!;
    }

    public class ValidatePromotionDTO
    {
        public string Code { get; set; } = null!;
        public int DiscountPercent { get; set; }
        public string Name { get; set; } = null!;
    }
}
