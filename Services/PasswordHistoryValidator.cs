using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using WebApplication1.Model;

namespace WebApplication1.Services
{
 public class PasswordHistoryValidator<TUser> : IPasswordValidator<TUser> where TUser : ApplicationUser
    {
 private const int PasswordHistoryLimit = 2;

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
        {
    if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(user.PasswordHistory))
     {
     return Task.FromResult(IdentityResult.Success);
          }

   try
     {
     var passwordHistory = JsonSerializer.Deserialize<List<string>>(user.PasswordHistory) ?? new List<string>();

      foreach (var oldPasswordHash in passwordHistory)
       {
      var passwordHasher = new PasswordHasher<TUser>();
   var result = passwordHasher.VerifyHashedPassword(user, oldPasswordHash, password);
     
    if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
       {
     return Task.FromResult(IdentityResult.Failed(new IdentityError
   {
   Code = "PasswordReused",
     Description = $"You cannot reuse any of your last {PasswordHistoryLimit} passwords."
          }));
         }
     }

       return Task.FromResult(IdentityResult.Success);
            }
    catch
    {
   return Task.FromResult(IdentityResult.Success); // If parsing fails, allow the password
}
  }

        public static void UpdatePasswordHistory(ApplicationUser user, string newPasswordHash)
      {
     var passwordHistory = string.IsNullOrEmpty(user.PasswordHistory)
         ? new List<string>()
          : JsonSerializer.Deserialize<List<string>>(user.PasswordHistory) ?? new List<string>();

    passwordHistory.Insert(0, newPasswordHash);

   if (passwordHistory.Count > PasswordHistoryLimit)
{
  passwordHistory = passwordHistory.Take(PasswordHistoryLimit).ToList();
            }

    user.PasswordHistory = JsonSerializer.Serialize(passwordHistory);
      user.LastPasswordChangeDate = DateTime.UtcNow;
      }
  }
}
