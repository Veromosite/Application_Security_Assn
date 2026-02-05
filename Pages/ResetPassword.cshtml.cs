using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _auditService;

        [BindProperty]
        public ResetPassword RPModel { get; set; } = new ResetPassword();

        public bool PasswordReset { get; set; }
        public string? ErrorMessage { get; set; }

        public ResetPasswordModel(
         UserManager<ApplicationUser> userManager,
  AuditService auditService)
 {
 _userManager = userManager;
        _auditService = auditService;
        }

        public IActionResult OnGet()
        {
   // Get verified email from session
            var email = HttpContext.Session.GetString("VerifiedPasswordResetEmail");
     
   if (string.IsNullOrEmpty(email))
       {
    return RedirectToPage("ForgotPassword");
}

            RPModel = new ResetPassword { Email = email };
return Page();
    }

  public async Task<IActionResult> OnPostAsync()
      {
          // Verify session again
  var sessionEmail = HttpContext.Session.GetString("VerifiedPasswordResetEmail");
       
    if (string.IsNullOrEmpty(sessionEmail) || sessionEmail != RPModel.Email)
       {
       return RedirectToPage("ForgotPassword");
       }

            if (!ModelState.IsValid)
            {
       return Page();
         }

            var user = await _userManager.FindByEmailAsync(RPModel.Email);
      if (user == null)
            {
         // Don't reveal that the user doesn't exist
   ErrorMessage = "An error occurred. Please try again.";
                return Page();
  }

     // Remove old password (force reset)
          var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
    {
     ErrorMessage = "An error occurred. Please try again.";
  return Page();
        }

            // Add new password
  var addPasswordResult = await _userManager.AddPasswordAsync(user, RPModel.Password);
     
            if (addPasswordResult.Succeeded)
            {
         // Update password history
       var passwordHash = _userManager.PasswordHasher.HashPassword(user, RPModel.Password);
          PasswordHistoryValidator<ApplicationUser>.UpdatePasswordHistory(user, passwordHash);
          await _userManager.UpdateAsync(user);

         await _auditService.LogAsync(user.Id, "Password Reset", "Password reset successfully via OTP");
  
        // Clear session
     HttpContext.Session.Remove("VerifiedPasswordResetEmail");
      
   PasswordReset = true;
     return Page();
        }

       foreach (var error in addPasswordResult.Errors)
            {
         ModelState.AddModelError("", error.Description);
     }

            return Page();
     }
    }
}
