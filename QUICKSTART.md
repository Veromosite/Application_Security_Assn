# Quick Start Configuration Guide

## ? Essential Setup (3 Minutes)

### Step 1: Get reCAPTCHA Keys (1 minute)
1. Visit: https://www.google.com/recaptcha/admin
2. Click "+" to create new site
3. Choose **reCAPTCHA v3**
4. Add domain: `localhost` for development
5. Copy your **Site Key** and **Secret Key**

### Step 2: Update appsettings.json (1 minute)

Open `appsettings.json` and update:

```json
{
  "RecaptchaSettings": {
    "SiteKey": "PASTE_YOUR_SITE_KEY_HERE",
    "SecretKey": "PASTE_YOUR_SECRET_KEY_HERE"
  },
  
  "Encryption": {
    "Key": "MySecure32CharacterKeyForAES!",
    "IV": "My16CharIVKey!!!"
  }
}
```

### Step 3: Run the Application (1 minute)

```bash
# In Visual Studio: Press F5
# Or in terminal:
dotnet run
```

Navigate to: `https://localhost:5001` (or the port shown in console)

## ?? Test the Application

### Test Flow 1: Registration & Login (with 2FA)

1. **Register**:
   - Go to `/Register`
   - Fill in all fields:
     - First Name: John
     - Last Name: Doe
     - Gender: Male
     - NRIC: S1234567A
     - Date of Birth: 2000-01-01
     - Email: john.doe@example.com
     - Password: Strong@Pass123
     - Confirm Password: Strong@Pass123
     - Who Am I: (Optional) Any text
   - Upload resume (optional - use any .pdf or .docx file)
   - Click Register

2. **Login**:
 - Go to `/Login`
   - Enter email and password
   - Check console output for 2FA code (email not configured)
   - Example console output:
     ```
     === EMAIL SENT ===
     To: john.doe@example.com
     Subject: Your 2FA Code - Ace Job Agency
     Body: Your verification code is: 123456
  ==================
```
   - Enter the 6-digit code on 2FA page

3. **View Profile**:
   - After successful login, you'll see the homepage
   - Your profile displays all information
   - NRIC is shown **decrypted** (but stored encrypted in DB)

### Test Flow 2: Password Security Features

#### Test Password History:
1. Login to your account
2. Click "Change Password" in navbar
3. Enter current password
4. Enter a new password
5. Change password
6. Try to change password back to the old one
7. **Expected**: Error - "You cannot reuse any of your last 2 passwords"

#### Test Account Lockout:
1. Logout
2. Go to Login
3. Enter correct email but WRONG password
4. Click Login (1st attempt)
5. Repeat with wrong password (2nd attempt)
6. Repeat with wrong password (3rd attempt)
7. **Expected**: Account locked for 5 minutes message

#### Test Password Strength:
1. Go to Register or Change Password
2. Start typing a password
3. **Expected**: Real-time strength indicator shows Weak/Medium/Strong

### Test Flow 3: Password Reset

1. Go to `/Login`
2. Click "Forgot Password?"
3. Enter your email
4. Check console output for reset link
5. Copy the reset link from console
6. Paste in browser
7. Enter new password
8. **Expected**: Password reset successfully

### Test Flow 4: View Audit Logs (Database)

1. Open SQL Server Object Explorer in Visual Studio
2. Connect to `(localdb)\ProjectModels`
3. Expand `AspNetAuth` database
4. Right-click `AuditLogs` table
5. Select "View Data"
6. **Expected**: See all logged activities:
   - Registration
   - Login attempts
   - 2FA verifications
   - Password changes
   - Logout events

## ?? Verify Security Features

### ? NRIC Encryption
1. Login and view profile - NRIC displays correctly
2. Go to database: `AspNetUsers` table
3. View `NRIC` column
4. **Expected**: Encrypted string (e.g., "aGJk2F3g...")

### ? XSS Protection
1. Register with WhoAmI: `<script>alert('XSS')</script>`
2. View profile
3. **Expected**: HTML tags displayed as text, not executed

### ? Session Timeout
1. Login
2. Wait 20 minutes (or change timeout to 1 minute in appsettings.json)
3. Try to navigate
4. **Expected**: Redirected to login

### ? Concurrent Login Prevention
1. Login from Chrome
2. Login from Edge/Firefox with same account
3. Try to use Chrome session
4. **Expected**: Chrome session invalidated (if AllowConcurrentLogins = false)

## ?? Database Tables Created

After running the application, check these tables in `AspNetAuth` database:

1. **AspNetUsers** - User accounts with:
   - NRIC (encrypted)
 - Password history
   - Session tokens
   - 2FA codes
   
2. **AuditLogs** - Security audit trail

3. **AspNetRoles**, **AspNetUserRoles**, etc. - Standard Identity tables

## ?? Common Issues & Fixes

### Issue: reCAPTCHA validation failed
**Fix**: 
- Make sure you got v3 keys (not v2)
- Check Site Key in appsettings.json matches
- For testing, the service allows empty keys in development

### Issue: Email not sending
**Fix**: 
- This is expected! Check console output instead
- For production, configure SMTP in appsettings.json

### Issue: Account locked
**Wait**: 5 minutes or change lockout duration in Program.cs

### Issue: Password won't accept
**Check**: 
- Minimum 12 characters
- At least one uppercase (A-Z)
- At least one lowercase (a-z)
- At least one number (0-9)
- At least one special character (@$!%*?&)

### Issue: File upload failed
**Check**:
- File is .pdf or .docx
- File size under 5MB
- `wwwroot/uploads/resumes` folder exists

## ?? For School Assignment Demonstration

### Show These Features:
1. ? Strong password policy (type weak password in registration)
2. ? Password strength indicator (real-time feedback)
3. ? NRIC encryption (show database vs display)
4. ? 2FA flow (complete login process)
5. ? Account lockout (3 wrong attempts)
6. ? Password history (try to reuse password)
7. ? Audit logs (show database records)
8. ? Custom error pages (navigate to /nonexistent)
9. ? XSS protection (WhoAmI with HTML tags)
10. ? reCAPTCHA (show validation on forms)
11. ? Session management (timeout demo)
12. ? Password reset (forgot password flow)

### Screenshots to Take:
1. Registration page with password strength
2. 2FA code entry page
3. Home page showing decrypted NRIC
4. Database showing encrypted NRIC
5. AuditLogs table with entries
6. Account lockout message
7. Password history error message
8. Custom 404 error page

## ?? Code Highlights for Presentation

### Show These Files:
1. `Services/EncryptionService.cs` - AES encryption implementation
2. `Services/PasswordHistoryValidator.cs` - Password history logic
3. `Pages/Login.cshtml.cs` - 2FA and lockout handling
4. `Services/AuditService.cs` - Audit logging
5. `Program.cs` - Security configuration

### Key Code Snippets to Explain:
```csharp
// Password policy configuration
options.Password.RequiredLength = 12;
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;

// Account lockout
options.Lockout.MaxFailedAccessAttempts = 3;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

// NRIC encryption
user.NRIC = _encryptionService.Encrypt(RModel.NRIC);

// Audit logging
await _auditService.LogAsync(user.Id, "Login", "User logged in successfully");
```

## ? Bonus Features Implemented

Beyond the requirements:
- ? Real-time password strength indicator
- ? Bootstrap 5 responsive design
- ? Bootstrap Icons
- ? Security headers (CSP, X-Frame-Options, etc.)
- ? Secure cookies (HttpOnly, Secure, SameSite)
- ? Session token validation
- ? IP address logging
- ? Comprehensive error handling
- ? Form validation (client & server)
- ? Age validation (18+ only)

---

**Ready to Run!** ??

Just update the reCAPTCHA keys and hit F5!
