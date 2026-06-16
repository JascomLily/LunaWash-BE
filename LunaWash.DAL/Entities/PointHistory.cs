using System;

namespace LunaWash.DAL.Entities
{
    public class PointHistory
    {
        public int Id { get; set; }

        
        public string UserId { get; set; } = null!;
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UserId")]
        public CustomerProfile CustomerProfile { get; set; } = null!;

        public int Points { get; set; }
        public int RemainingPoints { get; set; } 
        public string Description { get; set; } = string.Empty;
        public string? BookingId { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; } 
        public bool IsExpired { get; set; } = false; 
    }
}