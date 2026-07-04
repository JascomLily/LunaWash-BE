using System;

namespace LunaWash.DAL.Entities
{
    public class Banner
    {
        public string Id { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? Title { get; set; }
        public string? RedirectUrl { get; set; } // Can hold promoCode or a URL
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
