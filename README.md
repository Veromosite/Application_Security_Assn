# Ace Job Agency - Secure Web Application

A comprehensive ASP.NET Core Razor Pages web application implementing advanced security features for a job agency platform.

## ?? Security Features Implemented

### 1. Data Security
- **NRIC Encryption**: NRIC field encrypted using AES-256 encryption before storage
- **Secure File Upload**: Resume upload with validation (.pdf/.docx only, max 5MB)
- **SQL Injection Prevention**: Entity Framework with parameterized queries
- **XSS Protection**: HTML encoding for user-generated content (especially WhoAmI field)

### 2. Authentication & Authorization
- **Custom Identity Framework**: Extended ApplicationUser with additional fields
- **Strong Password Policy**:
  - Minimum 12 characters
  - Requires uppercase, lowercase, numbers, and special characters
  - Real-time password strength indicator
- **Password History**: Prevents reuse of last 2 passwords
- **Password Age Enforcement**:
  - Minimum age: 1 day (configurable)
  - Maximum age: 90 days (configurable)
- **Account Lockout**: 
  - Locks after 3 failed login attempts
  - 5-minute lockout period
  - Automatic recovery

### 3. Two-Factor Authentication (2FA)
- Email-based 2FA with 6-digit code
- 10-minute code expiration
- Secure code generation and validation

### 4. Password Reset
- Secure "Forgot Password" flow with email link
- Token-based password reset with expiration
- Password history validation on reset

### 5. Session Management
- Configurable session timeout (default: 20 minutes)
- Concurrent login detection and prevention
- Session token validation
- Automatic session invalidation

### 6. Bot Protection
- Google reCAPTCHA v3 integration
- Implemented on Register and Login pages
- Score-based validation (threshold: 0.5)

### 7. Audit Logging
- Comprehensive activity logging to database
- Tracks: Login, Logout, Registration, Password Changes, Failed Attempts
- Includes IP address and timestamp
- Stored in dedicated AuditLog table

### 8. Error Handling
- Custom error pages for 404 and 403
- No stack trace exposure in production
- Centralized error logging
- User-friendly error messages

### 9. Security Headers
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- X-XSS-Protection: 1; mode=block
- Referrer-Policy: strict-origin-when-cross-origin
- Content-Security-Policy (CSP)

## ?? Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- SQL Server LocalDB or SQL Server

## ?? Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository-url>
cd WebApplication1
```

### 2. Configure Settings

Update `appsettings.json` with your settings:

#### Encryption Keys
```json
"Encryption": {
  "Key": "YourSecure32CharacterKeyHere!@#$",
  "IV": "YourSecure16IV!!"
}
```
?? **IMPORTANT**: In production, store these in Azure Key Vault or secure configuration.

#### Email Settings
```json
"EmailSettings": {
  "SenderName": "Ace Job Agency",
  "SenderEmail": "noreply@acejobagency.com",
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "Username": "your-email@gmail.com",
  "Password": "your-app-password"
}
```

#### reCAPTCHA Settings
1. Get your reCAPTCHA v3 keys from: https://www.google.com/recaptcha/admin
2. Update configuration:
```json
"RecaptchaSettings": {
  "SiteKey": "your-site-key-here",
  "SecretKey": "your-secret-key-here"
}
```

### 3. Database Setup

```bash
# Restore packages
dotnet restore

# Create/Update database
dotnet ef database update --context AuthDbContext
```

### 4. Run the Application

```bash
dotnet run
```

Or press F5 in Visual Studio.

## ?? Project Structure

```
WebApplication1/
??? Model/
?   ??? ApplicationUser.cs          # Custom user model with security fields
?   ??? AuditLog.cs        # Audit logging model
?   ??? AuthDbContext.cs    # Database context
?   ??? AuthDbContextFactory.cs     # EF design-time factory
??? Services/
?   ??? EncryptionService.cs        # AES encryption service
?   ??? AuditService.cs   # Audit logging service
?   ??? EmailService.cs    # Email sending service
?   ??? RecaptchaService.cs         # reCAPTCHA validation
?   ??? PasswordHistoryValidator.cs # Password history validation
??? Middleware/
?   ??? SessionValidationMiddleware.cs # Session management
??? ViewModels/
?   ??? Register.cs # Registration view model
?   ??? Login.cs      # Login view models
?   ??? ForgotPassword.cs           # Password reset view models
?   ??? ChangePassword.cs         # Change password view model
??? Pages/
?   ??? Register.cshtml/cs          # Registration page
?   ??? Login.cshtml/cs             # Login page
?   ??? TwoFactorLogin.cshtml/cs    # 2FA verification
?   ??? ForgotPassword.cshtml/cs    # Password reset request
?   ??? ResetPassword.cshtml/cs     # Password reset page
?   ??? ChangePassword.cshtml/cs    # Change password page
?   ??? Index.cshtml/cs           # Home page (protected)
?   ??? Logout.cshtml/cs       # Logout handler
?   ??? Error.cshtml/cs     # Error page
?   ??? AccessDenied.cshtml/cs      # Access denied page
??? wwwroot/uploads/resumes/        # Resume upload directory
```

## ?? Database Schema

### ApplicationUser (extends IdentityUser)
- FirstName, LastName, Gender
- NRIC (encrypted)
- Email (unique)
- DateOfBirth
- ResumePath
- WhoAmI
- PasswordHistory (JSON array)
- LastPasswordChangeDate
- SessionToken, SessionTokenExpiry
- TwoFactorCode, TwoFactorCodeExpiry

### AuditLog
- Id
- UserId
- Action
- Timestamp
- IpAddress
- Details

## ?? Testing the Application

### Test Registration
1. Navigate to `/Register`
2. Fill in all required fields
3. Upload a resume (.pdf or .docx)
4. Complete reCAPTCHA
5. Verify strong password policy enforcement

### Test Login & 2FA
1. Navigate to `/Login`
2. Enter credentials
3. Complete reCAPTCHA
4. Check email/console for 2FA code
5. Enter 6-digit code on 2FA page

### Test Account Lockout
1. Attempt login with wrong password 3 times
2. Account should be locked for 5 minutes
3. Verify lockout message displayed

### Test Password History
1. Login successfully
2. Go to Change Password
3. Try to reuse a previous password
4. Verify error message

### Test Session Management
1. Login from one browser
2. Login from another browser (different session)
3. First session should be invalidated (if concurrent logins disabled)

## ??? Security Best Practices

1. **Encryption Keys**: Never commit encryption keys to source control
2. **Database**: Use environment-specific connection strings
3. **Secrets**: Use Azure Key Vault or User Secrets for sensitive data
4. **HTTPS**: Always use HTTPS in production
5. **Updates**: Keep all NuGet packages updated
6. **Logging**: Monitor audit logs regularly
7. **Backup**: Regular database backups

## ?? Configuration Options

### Password Policy (appsettings.json)
```json
"PasswordPolicy": {
  "MinimumAgeDays": 1,      // Minimum days before password can be changed
  "MaximumAgeDays": 90      // Maximum days before password must be changed
}
```

### Session Settings
```json
"SessionSettings": {
  "TimeoutMinutes": 20,        // Session timeout
  "AllowConcurrentLogins": false  // Allow multiple simultaneous sessions
}
```

## ?? Troubleshooting

### Email Not Sending
- Current implementation logs emails to console for development
- Configure SMTP settings for production use
- For Gmail: Use App Password, not regular password

### reCAPTCHA Not Working
- Verify Site Key is correctly set in appsettings.json
- Check browser console for JavaScript errors
- Ensure domain is registered with Google reCAPTCHA

### Database Connection Errors
- Verify SQL Server/LocalDB is running
- Check connection string in appsettings.json
- Run `dotnet ef database update` again

### NRIC Encryption Issues
- Ensure encryption key is exactly 32 characters
- Ensure IV is exactly 16 characters
- Check that EncryptionService is properly injected

## ?? Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- ASP.NET Core Identity
- SQL Server
- Bootstrap 5
- Google reCAPTCHA v3
- MailKit (Email)
- AES Encryption

## ?? User Fields

### Registration Form
- First Name (required)
- Last Name (required)
- Gender (required, dropdown)
- NRIC (required, format: S1234567A)
- Date of Birth (required, min age 18)
- Email (required, unique)
- Password (12+ chars, complex)
- Resume (.pdf/.docx, max 5MB)
- Who Am I (2000 chars max, allows special characters)

## ?? Security Checklist

- ? NRIC encrypted at rest (AES-256)
- ? Password policy enforced (12+ chars, complex)
- ? Password history (last 2 passwords)
- ? Password age management
- ? Account lockout (3 attempts, 5 min)
- ? 2FA via email
- ? Password reset via email
- ? Session timeout and management
- ? Concurrent login prevention
- ? reCAPTCHA v3 protection
- ? XSS protection (HTML encoding)
- ? SQL injection prevention (EF Core)
- ? Audit logging (all key actions)
- ? Custom error pages (no stack traces)
- ? Security headers
- ? Secure cookies (HttpOnly, Secure, SameSite)

## ?? Support

For issues or questions:
1. Check the troubleshooting section
2. Review audit logs for security events
3. Check application logs in console/logs folder

## ?? License

This project is for educational purposes (School Assignment - Web Security Module).

## ?? Important Notes

1. **Development Mode**: Email sending is currently configured for console output. Configure SMTP for production.
2. **Encryption Keys**: Change the default encryption keys before deployment.
3. **reCAPTCHA**: Register your domain and get your own keys.
4. **Database**: Secure your production database connection string.
5. **Session Keys**: Data protection keys are stored in ./keys folder - secure this in production.

---

**School Assignment**: NYP Application Security Module  
**Project**: Ace Job Agency - Secure Web Application  
**Framework**: ASP.NET Core Razor Pages (.NET 8)
