using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
  [DataType(DataType.Password)]
   [StringLength(100, MinimumLength = 12)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
     ErrorMessage = "Password must contain at least one lowercase, one uppercase, one number, and one special character")]
  [Display(Name = "New Password")]
  public string NewPassword { get; set; } = string.Empty;

        [Required]
      [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
  [Display(Name = "Confirm New Password")]
  public string ConfirmPassword { get; set; } = string.Empty;
    }
}
