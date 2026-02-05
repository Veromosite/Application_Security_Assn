using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class ForgotPasswordModel : PageModel
    {
    private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
  private readonly AuditService _auditService;

   [BindProperty]
        public ForgotPassword FPModel { get; set; } = new ForgotPassword();

        public bool EmailSent { get; set; }

        public ForgotPasswordModel(
  UserManager<ApplicationUser> userManager,
  EmailService emailService,
       AuditService auditService)
        {
       _userManager = userManager;
            _emailService = emailService;
   _auditService = auditService;
        }

      public void OnGet()
        {
        }

      public async Task<IActionResult> OnPostAsync()
        {
 if (!ModelState.IsValid)
            {
     return Page();
    }

            var user = await _userManager.FindByEmailAsync(FPModel.Email);
   
            // Don't reveal whether user exists
            EmailSent = true;

            if (user != null)
    {
           // Generate 6-digit OTP code
     var otpCode = new Random().Next(100000, 999999).ToString();
        
       // Store OTP code and expiry in user record
    user.TwoFactorCode = otpCode;
                user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

            // Send OTP via email
       await _emailService.Send2FACodeAsync(user.Email!, otpCode);
            
       await _auditService.LogAsync(user.Id, "Password Reset OTP Requested", "OTP code sent");
  
      // Store email in session for verification page
    HttpContext.Session.SetString("PasswordResetEmail", user.Email!);
      
                // Redirect to OTP verification page
                return RedirectToPage("VerifyPasswordResetOTP");
    }

    return Page();
        }
    }
}
