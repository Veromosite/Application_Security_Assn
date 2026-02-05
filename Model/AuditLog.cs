using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class AuditLog
    {
        [Key]
    public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
   [StringLength(100)]
        public string Action { get; set; } = string.Empty;

[Required]
    public DateTime Timestamp { get; set; }

  [StringLength(45)]
     public string? IpAddress { get; set; }

[StringLength(500)]
        public string? Details { get; set; }
    }
}
