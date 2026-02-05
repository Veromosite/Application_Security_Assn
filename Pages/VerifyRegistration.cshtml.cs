using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class VerifyRegistrationModel : PageModel
    {
private readonly UserManager<ApplicationUser> _userManager;
     private readonly AuditService _auditService;
 private readonly EmailService _emailService;

 [BindProperty]
 public TwoFactorLogin VModel { get; set; } = new TwoFactorLogin();

        public string? ErrorMessage { get; set; }
     public string? Email { get; set; }

   public VerifyRegistrationModel(
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
    var userId = HttpContext.Session.GetString("PendingRegistrationUserId");
    var email = HttpContext.Session.GetString("PendingRegistrationEmail");
     
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
   {
   return RedirectToPage("Register");
     }

     Email = email;
   return Page();
     }

      public async Task<IActionResult> OnPostAsync()
      {
   if (!ModelState.IsValid)
   {
       return Page();
       }

   var userId = HttpContext.Session.GetString("PendingRegistrationUserId");
     var email = HttpContext.Session.GetString("PendingRegistrationEmail");
       
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
  {
      return RedirectToPage("Register");
  }

Email = email;

  var user = await _userManager.FindByIdAsync(userId);
         if (user == null)
   {
     ModelState.AddModelError("", "User not found. Please register again.");
       return RedirectToPage("Register");
  }

    // Verify 2FA code
    if (user.TwoFactorCode != VModel.Code)
   {
  await _auditService.LogAsync(user.Id, "Failed Registration Verification", "Invalid code");
     ModelState.AddModelError("", "Invalid verification code.");
      return Page();
  }

    // Check if code expired
     if (user.TwoFactorCodeExpiry < DateTime.UtcNow)
   {
    await _auditService.LogAsync(user.Id, "Failed Registration Verification", "Expired code");
      
    // Delete the unverified user
   await _userManager.DeleteAsync(user);
  
  // Clear session
   HttpContext.Session.Remove("PendingRegistrationUserId");
       HttpContext.Session.Remove("PendingRegistrationEmail");
  
      ModelState.AddModelError("", "Verification code has expired. Please register again.");
     return RedirectToPage("Register");
     }

     // Mark email as confirmed
     user.EmailConfirmed = true;
  user.TwoFactorCode = null;
     user.TwoFactorCodeExpiry = null;
  await _userManager.UpdateAsync(user);

   // Clear session
     HttpContext.Session.Remove("PendingRegistrationUserId");
   HttpContext.Session.Remove("PendingRegistrationEmail");

    await _auditService.LogAsync(user.Id, "Registration Verified", "Email verified successfully");

     TempData["SuccessMessage"] = "Registration verified successfully! Please log in.";
  return RedirectToPage("Login");
     }

 public async Task<IActionResult> OnPostResendCodeAsync()
   {
       var userId = HttpContext.Session.GetString("PendingRegistrationUserId");
     if (string.IsNullOrEmpty(userId))
     {
       return RedirectToPage("Register");
 }

  var user = await _userManager.FindByIdAsync(userId);
     if (user == null)
      {
     return RedirectToPage("Register");
  }

      // Generate new 2FA code
   var newCode = new Random().Next(100000, 999999).ToString();
user.TwoFactorCode = newCode;
      user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);
       await _userManager.UpdateAsync(user);

   // Send email
       await _emailService.Send2FACodeAsync(user.Email!, newCode);

      await _auditService.LogAsync(user.Id, "Registration Code Resent", "New verification code sent");

    TempData["SuccessMessage"] = "Verification code resent! Check your email.";
   return RedirectToPage();
}
  }
}
