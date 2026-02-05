# ?? 2FA Email Configuration Guide

## ?? The Problem

**You're not receiving 2FA emails** because the application is configured for **development mode** - emails are logged to the console instead of being sent.

---

## ?? **QUICK FIX: Find Your 2FA Code in Console**

### **Step 1: Look in Visual Studio Output Window**

1. In Visual Studio, click **View** ? **Output** (or press `Ctrl + Alt + O`)
2. In the dropdown at top, select **"Debug"** or **"WebApplication1"**
3. After clicking "Login", scroll through the output
4. Look for this section:

```
=== EMAIL SENT (CONSOLE ONLY - SMTP NOT CONFIGURED) ===
To: your-email@example.com
Subject: Your 2FA Code - Ace Job Agency
Body: 
    <html>
       <body>
  <h2>Two-Factor Authentication Code</h2>
<p>Your verification code is: <strong>123456</strong></p>  ?? THIS IS YOUR CODE
 <p>This code will expire in 10 minutes.</p>
  </body>
      </html>
=======================================================
```

### **Step 2: Copy the 6-Digit Code**

The code is the 6 digits shown in `<strong>123456</strong>`

### **Step 3: Enter on 2FA Page**

Paste the code on the Two-Factor Authentication page.

---

## ? **PERMANENT FIX: Enable Real Email Sending**

Choose one of these options to send **actual emails** to your inbox:

---

## ?? **Option A: Use Gmail (Recommended for School Projects)**

### **Step 1: Get Gmail App Password**

1. Go to: **https://myaccount.google.com/apppasswords**
2. Sign in with your Gmail account
3. You may need to enable 2-Step Verification first: **https://myaccount.google.com/security**
4. Go back to App Passwords
5. Under "Select app" choose **"Mail"**
6. Under "Select device" choose **"Windows Computer"**
7. Click **"Generate"**
8. Copy the **16-character password** (format: `xxxx xxxx xxxx xxxx`)
9. **Remove the spaces** ? `xxxxxxxxxxxxxxxx`

### **Step 2: Update appsettings.json**

Replace the EmailSettings section in `appsettings.json`:

```json
"EmailSettings": {
  "SenderName": "Ace Job Agency",
  "SenderEmail": "your-actual-gmail@gmail.com",?? Your Gmail address
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": "587",
  "Username": "your-actual-gmail@gmail.com", ?? Your Gmail address
  "Password": "xxxxxxxxxxxxxxxx"  ?? 16-char app password (no spaces)
}
```

### **Step 3: Restart Application**

1. Stop the app (Shift + F5)
2. Press F5 to restart
3. Try logging in
4. **Email will be sent to your Gmail inbox!** ?

---

## ?? **Option B: Use Outlook/Hotmail**

### **Configuration:**

```json
"EmailSettings": {
  "SenderName": "Ace Job Agency",
  "SenderEmail": "your-outlook@outlook.com",
  "SmtpServer": "smtp-mail.outlook.com",
  "SmtpPort": "587",
  "Username": "your-outlook@outlook.com",
  "Password": "your-outlook-password"
}
```

**Note**: Outlook doesn't require app-specific passwords, use your regular password.

---

## ?? **Option C: Use Yahoo Mail**

### **Configuration:**

```json
"EmailSettings": {
  "SenderName": "Ace Job Agency",
  "SenderEmail": "your-yahoo@yahoo.com",
  "SmtpServer": "smtp.mail.yahoo.com",
  "SmtpPort": "587",
  "Username": "your-yahoo@yahoo.com",
  "Password": "your-app-password"
}
```

**Get Yahoo App Password**: https://login.yahoo.com/account/security

---

## ?? **Option D: Testing Only - Use Papercut SMTP**

**Papercut** is a fake SMTP server that shows emails in a desktop window (perfect for testing).

### **Step 1: Download Papercut**
- Download from: https://github.com/ChangemakerStudios/Papercut-SMTP/releases
- Install and run Papercut

### **Step 2: Configure appsettings.json**

```json
"EmailSettings": {
  "SenderName": "Ace Job Agency",
  "SenderEmail": "noreply@acejobagency.com",
  "SmtpServer": "localhost",
  "SmtpPort": "25",
  "Username": "",
  "Password": ""
}
```

### **Step 3: Update EmailService.cs**

For Papercut, you need to skip authentication:

```csharp
if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
{
  await client.AuthenticateAsync(smtpUsername, smtpPassword);
}
```

**Emails will appear in Papercut window!** No real emails sent.

---

## ?? **How to Test After Configuration**

### **Test 1: Login with 2FA**

1. Start your application (F5)
2. Go to `/Login`
3. Enter your registered email and password
4. Click "Login"
5. **Check your email inbox** (or Papercut if using that)
6. You should receive an email with subject: **"Your 2FA Code - Ace Job Agency"**
7. Copy the 6-digit code from the email
8. Enter it on the 2FA page
9. Login successful! ?

### **Test 2: Password Reset**

1. Go to `/Login`
2. Click "Forgot Password?"
3. Enter your email
4. Click "Send Reset Link"
5. **Check your email inbox**
6. Click the reset link in the email
7. Should open password reset page ?

---

## ?? **Important Security Notes**

### **For Development (Current Setup):**
? Console logging is **fine for testing**  
? Easier to debug  
? No email configuration needed  

### **For Production (School Submission):**
?? You **should configure real email** to meet assignment requirements  
?? **Never commit real passwords** to Git  
?? Use environment variables or Azure Key Vault in production  

---

## ?? **Secure Your Email Credentials**

### **Option 1: Use User Secrets (Recommended)**

Instead of putting password in `appsettings.json`:

```bash
# Right-click project ? Manage User Secrets
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
```

### **Option 2: Environment Variables**

```bash
# Windows (Command Prompt)
setx EmailSettings__Password "your-app-password"
setx EmailSettings__Username "your-email@gmail.com"

# Windows (PowerShell)
$env:EmailSettings__Password="your-app-password"
$env:EmailSettings__Username="your-email@gmail.com"
```

Then in code:
```csharp
var username = _configuration["EmailSettings:Username"];
var password = _configuration["EmailSettings:Password"];
```

---

## ?? **How It Works Now**

### **Before (Current - Console Only):**
```
User clicks Login 
  ? 2FA code generated
? Code logged to console
  ? User checks Output window
  ? User enters code
  ? Login successful
```

### **After (With Email Configured):**
```
User clicks Login 
  ? 2FA code generated
  ? Email sent to user's inbox
  ? User checks email
  ? User enters code from email
  ? Login successful
```

---

## ?? **For Your School Assignment**

### **What's Required:**
? **2FA via Email** - You need to demonstrate email-based 2FA

### **Acceptable for Demo:**
- ? **Console logging** (current setup) - Show lecturer the Output window
- ? **Papercut** - Show emails in Papercut window
- ? **Real email** - Show actual email received

### **Recommendation:**
Use **Papercut** for demo - it's professional, visual, and doesn't require exposing your real email password.

---

## ? **FAQ**

### **Q: Why am I not receiving emails?**
**A:** Email sending is disabled by default. It's logging to console instead. Check Visual Studio Output window.

### **Q: Do I need to configure email for the assignment?**
**A:** Technically no - you can demonstrate using console logs. But real email or Papercut looks more professional.

### **Q: Is it safe to put my Gmail password in appsettings.json?**
**A:** NO! Use an **App Password** (not your real Gmail password) and don't commit it to Git. Use User Secrets instead.

### **Q: What if I don't want to configure email?**
**A:** That's fine! Just check the **Output window** in Visual Studio for the 2FA code.

### **Q: The code expired, what do I do?**
**A:** Go back to login page and login again. A new code will be generated (valid for 10 minutes).

### **Q: Can I disable 2FA for testing?**
**A:** Not recommended as it's a requirement. Just use console logging - it's faster anyway!

---

## ?? **Quick Decision Guide**

| Scenario | Best Option |
|----------|-------------|
| **Quick testing** | Console logging (current - no config needed) |
| **School demo** | Papercut SMTP (looks professional) |
| **Personal use** | Gmail with App Password |
| **Production** | Professional email service (SendGrid, AWS SES) |

---

## ? **Immediate Solution**

**For Right Now (No Configuration Needed):**

1. ? Login to your application
2. ? Open **View ? Output** in Visual Studio
3. ? Select **"Debug"** in dropdown
4. ? Look for: `Your verification code is: <strong>123456</strong>`
5. ? Copy the 6-digit code
6. ? Enter on 2FA page
7. ? Login successful!

**Your application is working perfectly** - it's just using console logging instead of email sending. This is **intentional for development/testing**.

---

## ?? **Summary**

? **Current State**: Emails log to console (by design)  
? **Find Code**: Visual Studio Output window  
? **Enable Email**: Configure SMTP in appsettings.json  
? **For Demo**: Use Papercut or show Output window  
? **For Production**: Use Gmail App Password or professional service  

**Your 2FA is working correctly - you just need to look in the right place for the code!** ??

---

**Files Modified:**
- ? `appsettings.json` - Added email configuration template
- ? `Services/EmailService.cs` - Enabled real email sending when configured
