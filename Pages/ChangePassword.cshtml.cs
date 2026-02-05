using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
 private readonly UserManager<ApplicationUser> _userManager;
     private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly AuditService _auditService;
  private readonly IConfiguration _configuration;

 [BindProperty]
        public ChangePassword CPModel { get; set; } = new ChangePassword();

  public bool PasswordExpired { get; set; }
        public string? ErrorMessage { get; set; }
   public bool PasswordChanged { get; set; }

        public ChangePasswordModel(
    UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
 AuditService auditService,
   IConfiguration configuration)
        {
  _userManager = userManager;
   _signInManager = signInManager;
 _auditService = auditService;
    _configuration = configuration;
  }

        public async Task<IActionResult> OnGetAsync()
  {
      PasswordExpired = TempData["PasswordExpired"] as bool? ?? false;
   
  var user = await _userManager.GetUserAsync(User);
   if (user == null)
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

   var user = await _userManager.GetUserAsync(User);
      if (user == null)
 {
       return RedirectToPage("Login");
            }

            // Check minimum password age
       if (user.LastPasswordChangeDate.HasValue)
 {
          var minAgeDays = _configuration.GetValue<int>("PasswordPolicy:MinimumAgeDays", 1);
      var daysSinceChange = (DateTime.UtcNow - user.LastPasswordChangeDate.Value).TotalDays;

    if (daysSinceChange < minAgeDays)
      {
                ModelState.AddModelError("", $"You must wait at least {minAgeDays} day(s) before changing your password again.");
    return Page();
 }
   }

      // Change password
            var result = await _userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

  if (result.Succeeded)
      {
    // Update password history
  var passwordHash = _userManager.PasswordHasher.HashPassword(user, CPModel.NewPassword);
     PasswordHistoryValidator<ApplicationUser>.UpdatePasswordHistory(user, passwordHash);
   await _userManager.UpdateAsync(user);

    await _auditService.LogAsync(user.Id, "Password Changed", "User changed password");

       // Refresh sign-in
       await _signInManager.RefreshSignInAsync(user);

    PasswordChanged = true;
 TempData["SuccessMessage"] = "Password changed successfully!";
         
   return Page();
            }

   foreach (var error in result.Errors)
      {
    ModelState.AddModelError("", error.Description);
            }

   return Page();
        }
    }
}
