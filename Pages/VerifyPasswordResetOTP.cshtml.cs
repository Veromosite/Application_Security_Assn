using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class VerifyPasswordResetOTPModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _auditService;
        private readonly EmailService _emailService;

  [BindProperty]
        public VerifyPasswordResetOTP VModel { get; set; } = new VerifyPasswordResetOTP();

        public string? Email { get; set; }
        public string? ErrorMessage { get; set; }

     public VerifyPasswordResetOTPModel(
UserManager<ApplicationUser> userManager,
         AuditService auditService,
       EmailService emailService)
  {
    _userManager = userManager;
            _auditService = auditService;
  _emailService = emailService;
     }

        public IActionResult OnGet()
      {
            // Get email from session
        Email = HttpContext.Session.GetString("PasswordResetEmail");
         
        if (string.IsNullOrEmpty(Email))
  {
   return RedirectToPage("ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Email = HttpContext.Session.GetString("PasswordResetEmail");

            if (string.IsNullOrEmpty(Email))
            {
 return RedirectToPage("ForgotPassword");
            }

            if (!ModelState.IsValid)
         {
  return Page();
     }

            var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
            {
         return RedirectToPage("ForgotPassword");
         }

     // Verify OTP code
            if (user.TwoFactorCode != VModel.Code)
          {
                await _auditService.LogAsync(user.Id, "Failed Password Reset OTP", "Invalid code");
     ErrorMessage = "Invalid verification code.";
                return Page();
            }

        // Check if code expired
 if (user.TwoFactorCodeExpiry < DateTime.UtcNow)
          {
 await _auditService.LogAsync(user.Id, "Failed Password Reset OTP", "Expired code");
        ErrorMessage = "Verification code has expired. Please request a new code.";
        return Page();
            }

    // OTP verified successfully
// Clear the OTP code
        user.TwoFactorCode = null;
 user.TwoFactorCodeExpiry = null;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(user.Id, "Password Reset OTP Verified", "Code verified successfully");

            // Store verified email in session for reset password page
            HttpContext.Session.SetString("VerifiedPasswordResetEmail", Email);
            HttpContext.Session.Remove("PasswordResetEmail");

            // Redirect to reset password page
      return RedirectToPage("ResetPassword");
}

        public async Task<IActionResult> OnPostResendCodeAsync()
        {
         Email = HttpContext.Session.GetString("PasswordResetEmail");

   if (string.IsNullOrEmpty(Email))
      {
       return RedirectToPage("ForgotPassword");
            }

 var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
       // Generate new OTP code
      var otpCode = new Random().Next(100000, 999999).ToString();
    
      // Update user with new OTP
           user.TwoFactorCode = otpCode;
      user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);
      await _userManager.UpdateAsync(user);

    // Send new OTP via email
                await _emailService.Send2FACodeAsync(user.Email!, otpCode);
              
  await _auditService.LogAsync(user.Id, "Password Reset OTP Resent", "New OTP code sent");

     TempData["SuccessMessage"] = "A new verification code has been sent to your email.";
 }

       return RedirectToPage();
        }
    }
}
