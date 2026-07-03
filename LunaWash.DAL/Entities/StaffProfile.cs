using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities;

public partial class StaffProfile
{
    [Key]
    public string UserId { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    public int LeaveDays { get; set; }

    public int UsedLeaveDays { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
