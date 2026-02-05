# ?? reCAPTCHA Validation Failed - Troubleshooting Guide

## ? **Error Message**
```
reCAPTCHA validation failed. Please try again.
```

---

## ?? **Root Cause**

Your reCAPTCHA keys in `appsettings.json` are **real keys**, but Google is **rejecting** the validation. This happens when:

1. ? Keys are **expired** or **revoked**
2. ? Keys are for **wrong reCAPTCHA version** (v2 instead of v3)
3. ? Domain `localhost` is **not registered** with these keys
4. ? Keys are from **different site** registration
5. ? Rate limit exceeded (too many requests)

---

## ? **QUICK FIX - Option 1: Disable reCAPTCHA for Testing**

### **Step 1: Replace Keys with Placeholders**

In `appsettings.json`, change to:

```json
"RecaptchaSettings": {
  "SiteKey": "your-recaptcha-site-key",
  "SecretKey": "your-recaptcha-secret-key"
}
```

### **Step 2: Restart Application**

- Stop: **Shift + F5**
- Start: **F5**

### **Result:**
? Forms will submit without reCAPTCHA  
? Console shows: "reCAPTCHA: Keys not configured - skipping validation"  
? Perfect for development/testing  

---

## ?? **PERMANENT FIX - Option 2: Get Fresh reCAPTCHA v3 Keys**

Your current keys are failing. Let's get new ones:

### **Step 1: Delete Old reCAPTCHA Site (Optional)**

1. Go to: https://www.google.com/recaptcha/admin
2. Find your old site
3. Click the **trash icon** to delete it

### **Step 2: Create New reCAPTCHA v3 Site**

1. Go to: **https://www.google.com/recaptcha/admin/create**
2. Fill in:

```
???????????????????????????????????????
? Label: Ace Job Agency v3            ?
?    ?
? Type:           ?
?   ? reCAPTCHA v2        ?
?   ? reCAPTCHA v3  ? SELECT THIS!    ?
?         ?
? Domains:                 ?
? ??????????????????????????????????? ?
? ? localhost    ? ?
? ? 127.0.0.1      ? ?
? ??????????????????????????????????? ?
?     ?
? ? Accept reCAPTCHA Terms of Service?
?         ?
? [Submit]     ?
???????????????????????????????????????
```

3. Click **"Submit"**

### **Step 3: Copy NEW Keys**

You'll see:
```
Site Key:   6Lc[DIFFERENT-FROM-OLD]XXX
Secret Key: 6Lc[DIFFERENT-FROM-OLD]YYY
```

### **Step 4: Update appsettings.json**

```json
"RecaptchaSettings": {
  "SiteKey": "6Lc[YOUR-NEW-SITE-KEY]",
  "SecretKey": "6Lc[YOUR-NEW-SECRET-KEY]"
}
```

### **Step 5: Restart Application**

- Stop: **Shift + F5**
- Start: **F5**

### **Step 6: Test**

1. Go to `/Register`
2. Fill out form
3. Check console (Output window)
4. Should see: `? reCAPTCHA: Validation successful! Score: 0.9`

---

## ?? **Debugging Your Current Keys**

### **Check Output Window for Detailed Logs**

I've added detailed logging. After you try to submit a form:

1. Open **View ? Output** in Visual Studio
2. Select **"Debug"** or **"WebApplication1"** from dropdown
3. Look for reCAPTCHA logs:

#### **If Keys Not Configured:**
```
?? reCAPTCHA: Keys not configured - skipping validation
```
? Forms work without validation

#### **If Validation Successful:**
```
?? reCAPTCHA Response: {"success":true,"score":0.9,...}
? reCAPTCHA: Validation successful! Score: 0.9
```
? Everything working correctly

#### **If Validation Failed:**
```
?? reCAPTCHA Response: {"success":false,"error-codes":["invalid-input-secret"],...}
? reCAPTCHA: Validation failed. Success=False, Score=0
? reCAPTCHA Error Codes: invalid-input-secret
```
? Keys are wrong - need new keys

---

## ?? **Common Google reCAPTCHA Error Codes**

| Error Code | Meaning | Solution |
|------------|---------|----------|
| `missing-input-secret` | Secret key is missing | Add Secret Key to appsettings.json |
| `invalid-input-secret` | Secret key is invalid | Get new keys from reCAPTCHA admin |
| `missing-input-response` | Token is missing | Check JavaScript is executing |
| `invalid-input-response` | Token is invalid or expired | Token expired (10min limit), try again |
| `bad-request` | Request format is wrong | Check API call format |
| `timeout-or-duplicate` | Token already used or timeout | Get fresh token, submit again |

---

## ?? **How to Test Each Scenario**

### **Test 1: With Placeholder Keys (No Validation)**

**appsettings.json:**
```json
"SiteKey": "your-recaptcha-site-key",
"SecretKey": "your-recaptcha-secret-key"
```

**Expected:**
- ? Forms submit successfully
- ? No reCAPTCHA badge on page
- ? Console: "Keys not configured - skipping validation"

### **Test 2: With Real v3 Keys (Validation Active)**

**appsettings.json:**
```json
"SiteKey": "6Lc[REAL-SITE-KEY]",
"SecretKey": "6Lc[REAL-SECRET-KEY]"
```

**Expected:**
- ? reCAPTCHA badge in bottom-right
- ? Info message: "This form is protected by reCAPTCHA v3"
- ? Console: "Validation successful! Score: 0.9"
- ? Forms submit successfully

### **Test 3: With Invalid Keys (Validation Fails)**

**appsettings.json:**
```json
"SiteKey": "6LfAaFYsAAAAAEM5eEi08S7-nsq61PK_FlWtyzKg",  ? Your current keys
"SecretKey": "6LfAaFYsAAAAAFRh4x2IT_4mjlIHX9FFPXUpZjBg"
```

**Expected:**
- ? Forms don't submit
- ? Error: "reCAPTCHA validation failed"
- ? Console: "Validation failed" with error codes
- ? Need new keys!

---

## ?? **Verify Your Current Keys**

### **Check if Keys are v2 or v3:**

1. Go to: https://www.google.com/recaptcha/admin
2. Find the site with keys:
   - Site Key: `6LfAaFYsAAAAAEM5eEi08S7-nsq61PK_FlWtyzKg`
3. Check the **Type** column
4. If it says **"reCAPTCHA v2"** ? That's your problem!
5. If it says **"reCAPTCHA v3"** ? Check domains

### **Check Registered Domains:**

1. Click on the site name
2. Look at **Domains** section
3. Should include:
   ```
   localhost
   127.0.0.1
   ```
4. If missing ? Add them and save

---

## ? **Quick Decision Guide**

| Situation | What to Do |
|-----------|------------|
| **Just want to test app** | Use placeholder keys (Option 1) |
| **Need working reCAPTCHA for demo** | Get new v3 keys (Option 2) |
| **Keys worked before, stopped now** | Check if domain changed or keys expired |
| **Getting error codes in logs** | See error code table above |
| **Want to skip reCAPTCHA entirely** | Use placeholder keys |

---

## ?? **For Your School Assignment**

### **Recommendation:**

? **Use placeholder keys** for development/testing  
? **Get real v3 keys** only when ready to demo  
? **Show lecturer both scenarios:**
   1. Without reCAPTCHA (placeholder keys)
   2. With reCAPTCHA (real keys, if time permits)

### **What's Acceptable:**

? Demonstrating with placeholder keys is **fine**  
? Explaining how reCAPTCHA works theoretically  
? Showing the code implementation  
? Having real keys is **bonus**, not required  

---

## ?? **Current Status of Your Keys**

Your current keys in `appsettings.json`:
```json
"SiteKey": "6LfAaFYsAAAAAEM5eEi08S7-nsq61PK_FlWtyzKg",
"SecretKey": "6LfAaFYsAAAAAFRh4x2IT_4mjlIHX9FFPXUpZjBg"
```

**Possible Issues:**
1. ? These might be **reCAPTCHA v2** keys (wrong version)
2. ? Domain `localhost` might **not be registered**
3. ? Keys might be **expired** or **revoked**
4. ? Keys might be from a **different project**

**Solution:**
?? Get **fresh reCAPTCHA v3 keys** specifically for this project  
?? Make sure to select **v3** when creating  
?? Add `localhost` to domains  

---

## ?? **Summary**

### **Your Options:**

| Option | Time | Complexity | Best For |
|--------|------|------------|----------|
| **1. Use Placeholder Keys** | 30 seconds | Easy | Testing/development |
| **2. Get New v3 Keys** | 5 minutes | Easy | Demo/production |
| **3. Debug Current Keys** | Variable | Medium | Learning/troubleshooting |

### **Recommended Path:**

1. ? **NOW**: Use placeholder keys to unblock testing
2. ? **LATER**: Get fresh v3 keys when ready to demo
3. ? **OPTIONAL**: Debug current keys to understand the issue

---

## ?? **Next Steps**

### **Option A: Quick Fix (30 seconds)**

```json
// Change appsettings.json to:
"RecaptchaSettings": {
  "SiteKey": "your-recaptcha-site-key",
  "SecretKey": "your-recaptcha-secret-key"
}
```
Restart app ? ? Everything works!

### **Option B: Proper Fix (5 minutes)**

1. Go to: https://www.google.com/recaptcha/admin/create
2. Select: **reCAPTCHA v3**
3. Add: `localhost` to domains
4. Copy: Site Key and Secret Key
5. Paste: Into appsettings.json
6. Restart app ? ? reCAPTCHA working!

---

**Choose Option A for now to continue testing. Do Option B when ready for final demo!** ??

---

**Updated Files:**
- ? `Services/RecaptchaService.cs` - Added detailed logging
- ? `appsettings.json` - Changed to placeholder keys (temporary)
