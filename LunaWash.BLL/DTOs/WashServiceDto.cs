using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs
{
    public class WashServiceDto
    {
        public string Id { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<ServicePriceDto> Prices { get; set; } = new List<ServicePriceDto>();
    }

    public class WashServiceCreateDto
    {
        public string ServiceName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class WashServiceUpdateDto
    {
        public string ServiceName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServicePriceDto
    {
        public string Id { get; set; } = null!;
        public string VehicleTypeId { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public int PointsRewarded { get; set; }
    }

    public class ServicePriceCreateUpdateDto
    {
        public string VehicleTypeId { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public int PointsRewarded { get; set; }
    }
}
