using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
 public class ForgotPassword
    {
  [Required]
    [EmailAddress]
        [Display(Name = "Email Address")]
  public string Email { get; set; } = string.Empty;
    }

    public class VerifyPasswordResetOTP
    {
    [Required]
     [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Code must be 6 digits")]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;
    }

 public class ResetPassword
 {
      [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

 [Required]
  [DataType(DataType.Password)]
   [StringLength(100, MinimumLength = 12)]
 [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
     ErrorMessage = "Password must contain at least one lowercase, one uppercase, one number, and one special character")]
      [Display(Name = "New Password")]
  public string Password { get; set; } = string.Empty;

 [Required]
        [DataType(DataType.Password)]
 [Compare(nameof(Password))]
  [Display(Name = "Confirm Password")]
   public string ConfirmPassword { get; set; } = string.Empty;
    }
}
