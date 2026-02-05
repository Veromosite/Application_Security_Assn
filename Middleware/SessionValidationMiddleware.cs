using Microsoft.AspNetCore.Identity;
using WebApplication1.Model;

namespace WebApplication1.Middleware
{
    public class SessionValidationMiddleware
{
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
      {
  _next = next;
 }

  public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
 // Only check for authenticated users
  if (context.User.Identity?.IsAuthenticated == true)
            {
   var allowConcurrentLogins = configuration.GetValue<bool>("SessionSettings:AllowConcurrentLogins", false);
                
     if (!allowConcurrentLogins)
    {
 var sessionToken = context.Session.GetString("SessionToken");
   var user = await userManager.GetUserAsync(context.User);

      if (user != null && !string.IsNullOrEmpty(user.SessionToken))
   {
           // Check if session token matches
       if (sessionToken != user.SessionToken)
        {
        // Session token doesn't match - sign out
      context.Response.Redirect("/Login?error=session_expired");
        return;
     }

       // Check if session expired
     if (user.SessionTokenExpiry.HasValue && user.SessionTokenExpiry < DateTime.UtcNow)
        {
          context.Response.Redirect("/Login?error=session_expired");
       return;
      }
    }
      }
          }

await _next(context);
        }
    }

    public static class SessionValidationMiddlewareExtensions
    {
    public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
   {
      return builder.UseMiddleware<SessionValidationMiddleware>();
     }
    }
}
