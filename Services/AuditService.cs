using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class AuditService
    {
  private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(AuthDbContext context, IHttpContextAccessor httpContextAccessor)
        {
  _context = context;
 _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string userId, string action, string? details = null)
        {
var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var auditLog = new AuditLog
            {
        UserId = userId,
    Action = action,
        Timestamp = DateTime.UtcNow,
             IpAddress = ipAddress,
      Details = details
         };

       _context.AuditLogs.Add(auditLog);
  await _context.SaveChangesAsync();
        }
    }
}
