using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class TwoFactorLoginModel : PageModel
    {
  private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
 private readonly AuditService _auditService;
        private readonly IConfiguration _configuration;

    [BindProperty]
  public TwoFactorLogin TFModel { get; set; } = new TwoFactorLogin();

 public string? ErrorMessage { get; set; }

        public TwoFactorLoginModel(
    SignInManager<ApplicationUser> signInManager,
     UserManager<ApplicationUser> userManager,
       AuditService auditService,
  IConfiguration configuration)
   {
     _signInManager = signInManager;
 _userManager = userManager;
    _auditService = auditService;
 _configuration = configuration;
  }

    public IActionResult OnGet()
        {
   var userId = HttpContext.Session.GetString("TwoFactorUserId");
 if (string.IsNullOrEmpty(userId))
      {
       return RedirectToPage("Login");
    }
         return Page();
     }

     public async Task<IActionResult> OnPostAsync()
        {
    if (!ModelState.IsValid)
      {
    return Page();
    }

      var userId = HttpContext.Session.GetString("TwoFactorUserId");
   if (string.IsNullOrEmpty(userId))
      {
        return RedirectToPage("Login");
            }

   var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
    return RedirectToPage("Login");
      }

   // Verify 2FA code
       if (user.TwoFactorCode != TFModel.Code)
    {
          await _auditService.LogAsync(user.Id, "Failed 2FA Attempt", "Invalid code");
     ModelState.AddModelError("", "Invalid verification code.");
    return Page();
      }

     // Check if code expired
 if (user.TwoFactorCodeExpiry < DateTime.UtcNow)
     {
    await _auditService.LogAsync(user.Id, "Failed 2FA Attempt", "Expired code");
   ModelState.AddModelError("", "Verification code has expired. Please login again.");
    return RedirectToPage("Login");
     }

     // Clear 2FA code
     user.TwoFactorCode = null;
            user.TwoFactorCodeExpiry = null;
 await _userManager.UpdateAsync(user);

       // Sign in the user
   var rememberMe = bool.Parse(HttpContext.Session.GetString("RememberMe") ?? "false");
  await _signInManager.SignInAsync(user, rememberMe);

   // Clear session
     HttpContext.Session.Remove("TwoFactorUserId");
       HttpContext.Session.Remove("RememberMe");

       await _auditService.LogAsync(user.Id, "Successful Login", "2FA completed");

   return RedirectToPage("Index");
        }
    }
}
