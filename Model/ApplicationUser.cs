using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class ApplicationUser : IdentityUser
  {
    [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
   [StringLength(500)] // Encrypted NRIC will be longer
        public string NRIC { get; set; } = string.Empty;

   [Required]
    public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string? ResumePath { get; set; }

[StringLength(2000)]
        public string? WhoAmI { get; set; }

        // Password History - stores hashed passwords
        public string? PasswordHistory { get; set; } // JSON array of last 2 passwords

        // Password Age Management
      public DateTime? LastPasswordChangeDate { get; set; }

        // Session Management
        public string? SessionToken { get; set; }
        public DateTime? SessionTokenExpiry { get; set; }

        // Two-Factor Authentication
    public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiry { get; set; }
    }
}
