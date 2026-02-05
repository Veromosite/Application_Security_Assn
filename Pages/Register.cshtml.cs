using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.ViewModels;
using WebApplication1.Model;
using WebApplication1.Services;
using System.Text.Encodings.Web;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EncryptionService _encryptionService;
        private readonly AuditService _auditService;
        private readonly RecaptchaService _recaptchaService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Register RModel { get; set; } = new Register();

        public string? ErrorMessage { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EncryptionService encryptionService,
            AuditService auditService,
            RecaptchaService recaptchaService,
            EmailService emailService,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _encryptionService = encryptionService;
            _auditService = auditService;
            _recaptchaService = recaptchaService;
            _emailService = emailService;
            _environment = environment;
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

            // Verify reCAPTCHA
            var isRecaptchaValid = await _recaptchaService.VerifyRecaptchaAsync(RModel.RecaptchaToken);
            if (!isRecaptchaValid)
            {
                ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                return Page();
            }

            // Validate age (must be at least 18 years old)
            var age = DateTime.Today.Year - RModel.DateOfBirth.Year;
            if (RModel.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                ModelState.AddModelError("RModel.DateOfBirth", "You must be at least 18 years old to register.");
                return Page();
            }

            // Handle resume upload
            string? resumePath = null;
            if (RModel.Resume != null)
            {
                var allowedExtensions = new[] { ".pdf", ".docx" };
                var extension = Path.GetExtension(RModel.Resume.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("RModel.Resume", "Only PDF and DOCX files are allowed.");
                    return Page();
                }

                // Check file size (max 5MB)
                if (RModel.Resume.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("RModel.Resume", "File size must not exceed 5MB.");
                    return Page();
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "resumes");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{RModel.Resume.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await RModel.Resume.CopyToAsync(fileStream);
                }

                resumePath = $"/uploads/resumes/{uniqueFileName}";
            }

            // Create user with encrypted NRIC
            var user = new ApplicationUser
            {
                UserName = RModel.Email,
                Email = RModel.Email,
                FirstName = RModel.FirstName,
                LastName = RModel.LastName,
                Gender = RModel.Gender,
                NRIC = _encryptionService.Encrypt(RModel.NRIC), // Encrypt NRIC
                DateOfBirth = RModel.DateOfBirth,
                ResumePath = resumePath,
                WhoAmI = RModel.WhoAmI,
                LastPasswordChangeDate = DateTime.UtcNow,
                EmailConfirmed = false // Email not confirmed yet
            };

            var result = await _userManager.CreateAsync(user, RModel.Password);

            if (result.Succeeded)
            {
                // Update password history
                var passwordHash = _userManager.PasswordHasher.HashPassword(user, RModel.Password);
                PasswordHistoryValidator<ApplicationUser>.UpdatePasswordHistory(user, passwordHash);
                await _userManager.UpdateAsync(user);

                // Generate 2FA code for email verification
                var twoFactorCode = new Random().Next(100000, 999999).ToString();
                user.TwoFactorCode = twoFactorCode;
                user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                // Send verification email
                await _emailService.Send2FACodeAsync(user.Email!, twoFactorCode);

                // Log registration
                await _auditService.LogAsync(user.Id, "Registration", $"User registered: {user.Email} - awaiting email verification");

                // Store user ID in session for verification page
                HttpContext.Session.SetString("PendingRegistrationUserId", user.Id);
                HttpContext.Session.SetString("PendingRegistrationEmail", user.Email);

                // Redirect to verification page
                return RedirectToPage("VerifyRegistration");
            }

            foreach (var error in result.Errors)
            {
                // Skip the "Username already taken" error since we use Email as Username
                // We only want to show the email duplicate error
                if (error.Code == "DuplicateUserName")
                {
                    continue; // Skip this error
                }

                ModelState.AddModelError("", error.Description);
            }

            return Page();
        }
    }
}
