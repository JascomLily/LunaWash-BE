using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class CustomerVoucher
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string VoucherId { get; set; } = null!;

        public DateTime RedeemedDate { get; set; } = DateTime.UtcNow;

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedAt { get; set; }

        [StringLength(50)]
        public string? UsedAtBookingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual User Customer { get; set; } = null!;
        public virtual Voucher Voucher { get; set; } = null!;
        public virtual Booking? UsedAtBooking { get; set; }
    }
}
