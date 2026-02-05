using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        // Make RecaptchaToken optional - it will be validated server-side if reCAPTCHA is configured
        public string? RecaptchaToken { get; set; }
    }

    public class TwoFactorLogin
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
