using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs
{
    public class ServicePackageCreateUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public List<string> ServiceIds { get; set; } = new List<string>();
    }

    public class ServicePackageResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ServiceNames { get; set; } = new List<string>();
        public List<string> ServiceIds { get; set; } = new List<string>();
    }
}
