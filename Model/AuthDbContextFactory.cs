using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Model
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
     public AuthDbContext CreateDbContext(string[] args)
     {
      var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json")
      .Build();

   return new AuthDbContext(configuration);
 }
    }
}
