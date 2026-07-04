using System;

namespace LunaWash.BLL.DTOs
{
    public class BannerCreateUpdateDto
    {
        public string Url { get; set; } = null!; // maps to ImageUrl in DB
        public string? PromoCode { get; set; } // maps to RedirectUrl in DB
        public int Position { get; set; }
        public bool IsActive { get; set; }
    }

    public class BannerResponseDto
    {
        public string Id { get; set; } = null!;
        public string Url { get; set; } = null!; // maps to ImageUrl in DB
        public string? PromoCode { get; set; } // maps to RedirectUrl in DB
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
