using Microsoft.AspNetCore.Identity;
using WebApplication1.Model;
using WebApplication1.Services;
using Microsoft.AspNetCore.DataProtection;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Database Context
builder.Services.AddDbContext<AuthDbContext>();

// Identity Configuration with Custom User
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password Policy
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders()
.AddPasswordValidator<PasswordHistoryValidator<ApplicationUser>>();

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SessionSettings:TimeoutMinutes", 20));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SessionSettings:TimeoutMinutes", 20));
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Register Services
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<EmailService>();
// Use HttpClient for Recaptcha
builder.Services.AddHttpClient<RecaptchaService>();

// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
    .SetApplicationName("AceJobAgency");

// Add security headers (Antiforgery)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseSessionValidation();
app.UseAuthentication();
app.UseAuthorization();

// --- SECURITY HEADERS & CSP (UPDATED FOR RECAPTCHA V3) ---
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // CSP Configuration
    // NOTE: Added google.com and gstatic.com to connect-src, script-src, frame-src, img-src
    var csp = "default-src 'self'; " +
              "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.google.com https://www.gstatic.com; " +
              "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
              "font-src 'self' https://cdn.jsdelivr.net; " +
              "img-src 'self' data: https: https://www.google.com https://www.gstatic.com; " +
              "frame-src 'self' https://www.google.com https://www.gstatic.com; " +
              "connect-src 'self' https://www.google.com https://www.gstatic.com ws://localhost:* wss://localhost:* http://localhost:* https://localhost:*;";

    // In production, we might want to be stricter (remove unsafe-eval if possible), 
    // but for now, we use the same robust policy for v3 to work.
    context.Response.Headers.Append("Content-Security-Policy", csp);

    await next();
});

app.MapRazorPages();

app.Run();