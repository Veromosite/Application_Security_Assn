using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Model;
using WebApplication1.Services;
using System.Text.Encodings.Web;

namespace WebApplication1.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EncryptionService _encryptionService;

        public ApplicationUser? CurrentUser { get; set; }
        public string? DecryptedNRIC { get; set; }

        public IndexModel(
          ILogger<IndexModel> logger,
          UserManager<ApplicationUser> userManager,
          EncryptionService encryptionService)
        {
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                CurrentUser = user;
                // Decrypt NRIC for display
                DecryptedNRIC = _encryptionService.Decrypt(user.NRIC);
            }
        }
    }
}
