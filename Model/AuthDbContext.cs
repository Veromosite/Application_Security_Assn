using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        public AuthDbContext(IConfiguration configuration)
        {
  _configuration = configuration;
    }

 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
string connectionString = _configuration.GetConnectionString("AuthConnectionString") ?? throw new InvalidOperationException("Connection string 'AuthConnectionString' not found.");
        optionsBuilder.UseSqlServer(connectionString);
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    base.OnModelCreating(modelBuilder);

 // Configure AuditLog
      modelBuilder.Entity<AuditLog>(entity =>
 {
    entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.Timestamp);
      });
  }
    }
}
