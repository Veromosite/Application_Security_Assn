# ?? Testing the New OTP Password Reset Flow

## Quick Test Steps

### ? **Step 1: Request Password Reset**
1. Stop your app (if running) and restart it
2. Navigate to `/Login`
3. Click **"Forgot Password?"**
4. Enter a registered email address
5. Click **"Send Verification Code"**
6. ? You should see: "Verification Code Sent!" message

### ? **Step 2: Check Console for OTP**
Look in your **Visual Studio Output Console**:
```
=== EMAIL SENT (CONSOLE ONLY - SMTP NOT CONFIGURED) ===
To: your-email@example.com
Subject: Your 2FA Code - Ace Job Agency
Body: Your verification code is: 123456
=======================================================
```
?? **Copy the 6-digit code** (e.g., `123456`)

### ? **Step 3: Verify OTP**
1. You'll be automatically redirected to OTP verification page
2. Enter the 6-digit code you copied
3. Click **"Verify Code"**
4. ? You should be redirected to Reset Password page

### ? **Step 4: Reset Password**
1. Your email should be displayed at the top
2. Enter a new password that meets requirements:
   - At least 12 characters
   - Contains uppercase AND lowercase
   - Contains at least one number
   - Contains at least one special character (@$!%*?&)
3. Confirm the password
4. Watch the **password strength indicator**
5. Click **"Reset Password"**
6. ? You should see: "Password Reset Successful!" message

### ? **Step 5: Login with New Password**
1. Click **"Go to Login"**
2. Enter your email and **NEW** password
3. Complete 2FA (check console for login code)
4. ? You should successfully login!

---

## ?? Advanced Test Cases

### **Test Case A: Invalid OTP**
1. Request password reset
2. Enter wrong code (e.g., `000000`)
3. ? **Expected:** "Invalid verification code" error
4. ? Still on verification page

### **Test Case B: Expired OTP**
1. Request password reset
2. In code, temporarily change expiry to 1 second
3. Wait 2 seconds
4. Enter the code
5. ? **Expected:** "Verification code has expired" error

### **Test Case C: Resend Code**
1. Request password reset
2. On verification page, click **"Resend Code"**
3. Check console for **NEW** code
4. ? **Expected:** 
   - Success message shown
   - New code generated and sent
   - Old code invalid

### **Test Case D: Session Security**
1. Request password reset
2. Copy the URL of verification page
3. Open new browser tab
4. Paste the URL
5. ?? **Expected:** Redirected to Forgot Password (no session)

### **Test Case E: Direct Reset Password Access**
1. Try to navigate directly to `/ResetPassword`
2. ?? **Expected:** Redirected to `/ForgotPassword`
3. (Can only access after OTP verification)

### **Test Case F: Password Policy Enforcement**
Try these passwords (should all fail):
- `short` - Too short
- `alllowercase123!` - No uppercase
- `ALLUPPERCASE123!` - No lowercase
- `NoNumbers!` - No numbers
- `NoSpecialChar123` - No special characters

? **Expected:** Validation error messages

### **Test Case G: Password History**
1. Reset password to `NewPassword123!`
2. Login successfully
3. Request password reset again
4. Try to reset to `NewPassword123!` (same password)
5. ? **Expected:** "You cannot reuse any of your last 2 passwords"

---

## ?? What to Check in Database

### **After Requesting OTP:**
Open SQL Server Object Explorer ? `AspNetUsers` table:
- ? `TwoFactorCode` should contain 6-digit code
- ? `TwoFactorCodeExpiry` should be ~10 minutes in future

### **After Verifying OTP:**
Check `AspNetUsers` table again:
- ? `TwoFactorCode` should be NULL
- ? `TwoFactorCodeExpiry` should be NULL

### **After Resetting Password:**
Check `AspNetUsers` table:
- ? `PasswordHash` should be different
- ? `PasswordHistory` should contain previous hash
- ? `LastPasswordChangeDate` should be updated

### **Check Audit Logs:**
Open `AuditLogs` table:
- ? "Password Reset OTP Requested"
- ? "Password Reset OTP Verified"
- ? "Password Reset" (via OTP)

---

## ?? Expected Console Output

### **When Requesting OTP:**
```
=== EMAIL SENT (CONSOLE ONLY - SMTP NOT CONFIGURED) ===
To: john.doe@example.com
Subject: Your 2FA Code - Ace Job Agency
Body: 
    <html>
     <body>
    <h2>Two-Factor Authentication Code</h2>
       <p>Your verification code is: <strong>123456</strong></p>
   <p>This code will expire in 10 minutes.</p>
    <p>If you did not request this code, please ignore this email.</p>
  </body>
      </html>
=======================================================
```

### **When Resending OTP:**
```
=== EMAIL SENT (CONSOLE ONLY - SMTP NOT CONFIGURED) ===
To: john.doe@example.com
Subject: Your 2FA Code - Ace Job Agency
Body: 
    [... same format but NEW CODE ...]
    <p>Your verification code is: <strong>987654</strong></p>
=======================================================
```

---

## ? Success Checklist

- [ ] Can request password reset
- [ ] OTP code appears in console
- [ ] Can verify OTP code
- [ ] Can reset password after OTP verification
- [ ] Can login with new password
- [ ] Invalid OTP shows error
- [ ] Resend code works
- [ ] Session security prevents direct page access
- [ ] Password policy enforced
- [ ] Password history enforced
- [ ] Audit logs created for all actions

---

## ?? Troubleshooting

### **Issue: "Redirected to ForgotPassword"**
**Cause:** Session expired or missing  
**Fix:** Request password reset again (start from step 1)

### **Issue: "Invalid verification code"**
**Cause:** Wrong code entered or code expired  
**Fix:** Click "Resend Code" to get new code

### **Issue: "Password must contain..."**
**Cause:** Password doesn't meet requirements  
**Fix:** Follow password requirements shown on page

### **Issue: Console shows no OTP**
**Cause:** Email service not working  
**Fix:** Check `Services/EmailService.cs` and console output

### **Issue: Database error**
**Cause:** Database not updated  
**Fix:** Run migration: `dotnet ef database update`

---

## ?? Need Help?

If you encounter issues:
1. Check console output for errors
2. Check `AuditLogs` table for activity
3. Verify session is active
4. Ensure database is up to date
5. Review `PASSWORD_RESET_OTP_IMPLEMENTATION.md`

---

**Happy Testing! ??**
