using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;

namespace WebApplication1.Pages
{
    public class LogoutModel : PageModel
    {
  private readonly SignInManager<ApplicationUser> _signInManager;
      private readonly UserManager<ApplicationUser> _userManager;
 private readonly AuditService _auditService;

 public LogoutModel(
  SignInManager<ApplicationUser> signInManager,
   UserManager<ApplicationUser> userManager,
 AuditService auditService)
   {
     _signInManager = signInManager;
  _userManager = userManager;
   _auditService = auditService;
 }

    public async Task<IActionResult> OnGetAsync()
     {
            var user = await _userManager.GetUserAsync(User);
       if (user != null)
 {
      await _auditService.LogAsync(user.Id, "Logout", "User logged out");
    }

   await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
  
  return RedirectToPage("Login");
  }
    }
}
