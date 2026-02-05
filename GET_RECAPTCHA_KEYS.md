# ?? How to Get reCAPTCHA v3 Keys

## ?? Quick Guide to Get Your Keys

### **Step 1: Go to reCAPTCHA Admin Console**

Visit: **https://www.google.com/recaptcha/admin/create**

(You'll need to be signed in with your Google account)

---

### **Step 2: Register a New Site**

Fill out the registration form:

#### **1. Label**
```
Enter: Ace Job Agency
```
(This is just a friendly name for you to identify the site)

#### **2. reCAPTCHA Type**
```
Select: reCAPTCHA v3
```
?? **IMPORTANT**: Choose **v3**, NOT v2!
- v2 = Shows "I'm not a robot" checkbox (? Don't use this)
- v3 = Invisible, score-based (? Use this one!)

#### **3. Domains**
Add these domains (one per line):
```
localhost
127.0.0.1
```

For production later, also add:
```
yourdomain.com
www.yourdomain.com
```

#### **4. Owners**
```
Your Google email (auto-filled)
```
You can add other Google accounts if needed.

#### **5. Accept Terms**
```
?? Accept the reCAPTCHA Terms of Service
```

#### **6. Send Alerts**
```
?? Send alerts to owners (optional but recommended)
```

---

### **Step 3: Submit**

Click the **"Submit"** button at the bottom.

---

### **Step 4: Copy Your Keys**

You'll see a page with your keys:

```
Site Key:   6LcXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
Secret Key: 6LcYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
```

**Site Key**: Used in your HTML/JavaScript (public)  
**Secret Key**: Used in your server-side code (private)

---

### **Step 5: Update Your appsettings.json**

Replace the placeholder keys in your `appsettings.json`:

```json
"RecaptchaSettings": {
  "SiteKey": "6LcXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",    ?? Paste Site Key here
  "SecretKey": "6LcYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"  ?? Paste Secret Key here
}
```

---

### **Step 6: Restart Your Application**

1. Stop the app (Shift + F5)
2. Start the app (F5)
3. reCAPTCHA is now active! ?

---

## ?? **Visual Guide**

### **What the reCAPTCHA Admin Page Looks Like:**

```
???????????????????????????????????????????????
?  Register a new site      ?
???????????????????????????????????????????????
?      ?
?  Label  ?
?  ???????????????????????????????????????   ?
?  ? Ace Job Agency    ?   ?
?  ???????????????????????????????????????   ?
?    ?
?  reCAPTCHA type          ?
?  ? reCAPTCHA v2         ?
?  ? reCAPTCHA v3  ? SELECT THIS!      ?
?  ? reCAPTCHA Android         ?
?          ?
?  Domains          ?
?  ???????????????????????????????????????   ?
?  ? localhost           ?   ?
?  ? 127.0.0.1       ?   ?
?  ???????????????????????????????????????   ?
?    ?
?  ? Accept the reCAPTCHA Terms of Service   ?
?  ? Send alerts to owners    ?
?        ?
?  [Submit]   ?
???????????????????????????????????????????????
```

---

## ?? **Testing After Configuration**

### **Test on Register Page:**

1. Go to: `http://localhost:5000/Register`
2. You should see: "??? This form is protected by reCAPTCHA v3"
3. Fill out the form
4. Click "Register"
5. **reCAPTCHA runs invisibly in background** ?
6. Form submits successfully

### **Test on Login Page:**

1. Go to: `http://localhost:5000/Login`
2. You should see: "??? This form is protected by reCAPTCHA v3"
3. Enter credentials
4. Click "Login"
5. **reCAPTCHA validates invisibly** ?
6. Proceeds to 2FA page

---

## ?? **Verifying reCAPTCHA is Working**

### **In Browser:**

1. Open Developer Tools (F12)
2. Go to **Console** tab
3. You should see: `grecaptcha.execute()` calls
4. Look for: reCAPTCHA badge in bottom-right corner of page

### **On Server:**

1. Check Visual Studio **Output** window
2. After form submit, you should see reCAPTCHA validation logs
3. If validation fails, you'll see an error message

---

## ?? **reCAPTCHA Admin Console Features**

Once registered, you can access your site at:
**https://www.google.com/recaptcha/admin**

### **What You Can Do:**

? **View Analytics**
- See how many requests were made
- View score distribution
- Monitor suspicious activity

? **View Settings**
- See your site and secret keys
- Add/remove domains
- Configure alert preferences

? **View Metrics**
- Daily request volume
- Score breakdown
- Pass/fail rates

---

## ?? **Security Best Practices**

### **Site Key (Public)**
? Can be in HTML/JavaScript  
? Can be in Git repository  
? Visible to users in browser  

### **Secret Key (Private)**
? NEVER commit to Git  
? NEVER expose in client-side code  
? NEVER share publicly  
? Keep in appsettings.json (not committed)  
? Use User Secrets or Environment Variables  

---

## ?? **For Your School Assignment**

### **Required for Assignment:**
? reCAPTCHA v3 on Register page  
? reCAPTCHA v3 on Login page  
? Server-side validation  

### **For Demonstration:**
1. ? Show the reCAPTCHA badge on page
2. ? Show the info message: "This form is protected by reCAPTCHA v3"
3. ? Show successful form submission
4. ? (Optional) Show admin console analytics

---

## ?? **Understanding reCAPTCHA Scores**

When you submit a form, Google gives a **score from 0.0 to 1.0**:

| Score | Meaning | Action |
|-------|---------|--------|
| 0.9 - 1.0 | Definitely human | ? Allow |
| 0.7 - 0.9 | Very likely human | ? Allow |
| 0.5 - 0.7 | Probably human | ? Allow (default threshold) |
| 0.3 - 0.5 | Suspicious | ?? Challenge or deny |
| 0.0 - 0.3 | Likely bot | ? Deny |

**Your app uses threshold 0.5** (see `RecaptchaService.cs`)

---

## ?? **Alternative: Test Without Real Keys**

If you don't want to set up reCAPTCHA right now:

### **Your App Already Works Without It!**

? Forms submit successfully  
? Validation is skipped when keys are placeholders  
? Console shows: "reCAPTCHA not configured"  

This is **fine for development/testing**. Just get real keys before submitting your assignment.

---

## ? **Troubleshooting**

### **Problem: "reCAPTCHA validation failed"**

**Solutions:**
1. Check Site Key is correct in appsettings.json
2. Check domain is registered (localhost)
3. Check you selected **v3**, not v2
4. Clear browser cache and try again

### **Problem: "Invalid site key"**

**Solutions:**
1. Make sure you copied the entire key
2. No extra spaces before/after the key
3. Site Key goes in "SiteKey", not "SecretKey"

### **Problem: "Domain not registered"**

**Solutions:**
1. Add `localhost` to domains in reCAPTCHA admin
2. Add `127.0.0.1` to domains
3. Wait a few minutes for changes to propagate

---

## ?? **Quick Setup Checklist**

- [ ] Go to https://www.google.com/recaptcha/admin/create
- [ ] Choose **reCAPTCHA v3**
- [ ] Add label: "Ace Job Agency"
- [ ] Add domains: localhost, 127.0.0.1
- [ ] Accept terms
- [ ] Click Submit
- [ ] Copy **Site Key**
- [ ] Copy **Secret Key**
- [ ] Paste both in `appsettings.json`
- [ ] Restart application
- [ ] Test Register page
- [ ] Test Login page
- [ ] Verify reCAPTCHA badge appears
- [ ] Verify forms submit successfully

---

## ?? **Important Links**

| Resource | URL |
|----------|-----|
| **Register New Site** | https://www.google.com/recaptcha/admin/create |
| **Manage Sites** | https://www.google.com/recaptcha/admin |
| **Documentation** | https://developers.google.com/recaptcha/docs/v3 |
| **Score Interpretation** | https://developers.google.com/recaptcha/docs/v3#interpreting_the_score |
| **FAQ** | https://developers.google.com/recaptcha/docs/faq |

---

## ?? **Summary**

### **To Get Keys:**
1. ? Visit: https://www.google.com/recaptcha/admin/create
2. ? Select: **reCAPTCHA v3**
3. ? Add domains: localhost
4. ? Copy your keys

### **To Configure App:**
1. ? Paste keys in `appsettings.json`
2. ? Restart application
3. ? Test forms

### **Time Required:**
?? **5 minutes** to register and configure

---

**Ready? Go to https://www.google.com/recaptcha/admin/create and get your keys now!** ??
