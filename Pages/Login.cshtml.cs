using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _auditService;
        private readonly EmailService _emailService;
        private readonly RecaptchaService _recaptchaService;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public Login LModel { get; set; } = new Login();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            AuditService auditService,
            EmailService emailService,
            RecaptchaService recaptchaService,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _auditService = auditService;
            _emailService = emailService;
            _recaptchaService = recaptchaService;
            _configuration = configuration;
        }

        public void OnGet()
        {
            // Retrieve success message if redirected from Register
            SuccessMessage = TempData["SuccessMessage"] as string;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Verify reCAPTCHA v3
            // The service checks if the token is valid AND if the score is high enough (> 0.5)
            var isRecaptchaValid = await _recaptchaService.VerifyRecaptchaAsync(LModel.RecaptchaToken);

            if (!isRecaptchaValid)
            {
                ModelState.AddModelError("", "reCAPTCHA validation failed. You might be a bot.");
                return Page();
            }

            // 2. Find User
            var user = await _userManager.FindByEmailAsync(LModel.Email);
            if (user == null)
            {
                await _auditService.LogAsync("Unknown", "Failed Login Attempt", $"Email: {LModel.Email}");
                ModelState.AddModelError("", "Invalid login attempt.");
                return Page();
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                await _auditService.LogAsync(user.Id, "Failed Login Attempt", "Email not verified");
                ModelState.AddModelError("", "Please verify your email before logging in. Check your inbox for the verification code.");
                return Page();
            }

            // 3. Check Lockout
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingTime = lockoutEnd.HasValue ? (lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes : 0;

                await _auditService.LogAsync(user.Id, "Locked Out Login Attempt", "Account locked");
                ModelState.AddModelError("", $"Account is locked. Please try again in {Math.Ceiling(remainingTime)} minutes.");
                return Page();
            }

            // 4. Check Password Age
            if (user.LastPasswordChangeDate.HasValue)
            {
                var maxAgeDays = _configuration.GetValue<int>("PasswordPolicy:MaximumAgeDays", 90);
                var daysSinceChange = (DateTime.UtcNow - user.LastPasswordChangeDate.Value).TotalDays;

                if (daysSinceChange > maxAgeDays)
                {
                    TempData["PasswordExpired"] = true;
                    TempData["UserId"] = user.Id;
                    return RedirectToPage("ChangePassword");
                }
            }

            // 5. Attempt Sign In
            var result = await _signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Manage Session / Concurrent Logins
                var allowConcurrentLogins = _configuration.GetValue<bool>("SessionSettings:AllowConcurrentLogins", false);
                if (!allowConcurrentLogins)
                {
                    var sessionToken = Guid.NewGuid().ToString();
                    user.SessionToken = sessionToken;
                    user.SessionTokenExpiry = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("SessionSettings:TimeoutMinutes", 20));
                    await _userManager.UpdateAsync(user);
                    HttpContext.Session.SetString("SessionToken", sessionToken);
                }

                // Generate 2FA Code
                var twoFactorCode = new Random().Next(100000, 999999).ToString();
                user.TwoFactorCode = twoFactorCode;
                user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                await _emailService.Send2FACodeAsync(user.Email!, twoFactorCode);

                // Sign out immediately (require 2FA to complete login)
                await _signInManager.SignOutAsync();

                HttpContext.Session.SetString("TwoFactorUserId", user.Id);
                HttpContext.Session.SetString("RememberMe", LModel.RememberMe.ToString());

                await _auditService.LogAsync(user.Id, "Login Initiated", "2FA code sent");

                return RedirectToPage("TwoFactorLogin");
            }

            if (result.IsLockedOut)
            {
                await _auditService.LogAsync(user.Id, "Account Locked", "Too many failed login attempts");
                ModelState.AddModelError("", "Account locked due to multiple failed login attempts. Please try again later.");
                return Page();
            }

            // Failed
            await _auditService.LogAsync(user.Id, "Failed Login Attempt", "Invalid password");
            ModelState.AddModelError("", "Invalid login attempt.");
            return Page();
        }
    }
}