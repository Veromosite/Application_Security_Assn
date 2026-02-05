# ?? Database Connection Error Fix

## ? Error Message

```
SqlException: Cannot open database "AspNetAuth" requested by the login. 
The login failed.
Login failed for user 'BELANDON\User'.
```

## ?? Root Cause

The **database was deleted or never created**. The application is trying to connect to a database called `AspNetAuth` that doesn't exist in your SQL Server LocalDB.

## ? Solution - Follow These Steps IN ORDER

### Step 1: Stop Your Running Application ?? CRITICAL

**You MUST stop the application first!** The build is failing because the app is still running.

In Visual Studio:
1. Click the **red square "Stop"** button, OR
2. Press **Shift + F5**

### Step 2: Rebuild the Application

Once stopped, rebuild:

```bash
dotnet build
```

OR press **Ctrl + Shift + B** in Visual Studio

### Step 3: Recreate the Database

Run this command in the terminal:

```bash
dotnet ef database update --context AuthDbContext
```

This will:
- ? Create the `AspNetAuth` database
- ? Apply all migrations
- ? Create all tables (AspNetUsers, AuditLogs, etc.)

### Step 4: Verify Database Creation

Check that the database was created:

1. Open **SQL Server Object Explorer** in Visual Studio
2. Expand **(localdb)\ProjectModels**
3. You should see **AspNetAuth** database
4. Expand it to see tables:
   - AspNetUsers
   - AspNetRoles
   - AuditLogs
   - etc.

### Step 5: Restart Your Application

Press **F5** in Visual Studio

---

## ?? Alternative: If Database Update Fails

If `dotnet ef database update` fails, try this complete reset:

### Option A: Delete and Recreate

```bash
# Delete the database
dotnet ef database drop --context AuthDbContext --force

# Recreate it
dotnet ef database update --context AuthDbContext
```

### Option B: Manual Database Creation (If EF Tools Don't Work)

1. Open **SQL Server Object Explorer** in Visual Studio
2. Right-click on **(localdb)\ProjectModels**
3. Select **New Query**
4. Run this SQL:

```sql
CREATE DATABASE AspNetAuth;
GO
```

5. Then run the migrations:

```bash
dotnet ef database update --context AuthDbContext
```

---

## ?? Additional Issue Fixed: Duplicate RecaptchaToken

I also fixed a code error in `ViewModels/Login.cs`:

### Before (WRONG - Duplicate Property):
```csharp
[Required]
public string RecaptchaToken { get; set; } = string.Empty;

public string? RecaptchaToken { get; set; }  // ? DUPLICATE!
```

### After (CORRECT):
```csharp
// Make RecaptchaToken optional
public string? RecaptchaToken { get; set; }  // ? Single property, nullable
```

---

## ? Quick Fix Summary

1. **STOP** the running application (Shift + F5)
2. **RUN**: `dotnet build`
3. **RUN**: `dotnet ef database update --context AuthDbContext`
4. **START** the application (F5)

---

## ?? Verification Checklist

After following the steps:

- [ ] Application stopped successfully
- [ ] Build completed without errors
- [ ] Database migration succeeded
- [ ] Database `AspNetAuth` visible in SQL Server Object Explorer
- [ ] Tables created (AspNetUsers, AuditLogs, etc.)
- [ ] Application starts without errors
- [ ] Can access `/Register` page
- [ ] Can submit registration form

---

## ?? Common Mistakes to Avoid

### ? Don't Skip Stopping the Application
If you try to build while the app is running, you'll get file lock errors.

### ? Don't Delete Database Files Manually
Use `dotnet ef database drop` instead of deleting .mdf/.ldf files.

### ? Don't Run Update While App Is Running
Stop the app before running migrations.

---

## ?? What Happened?

The database was likely deleted when:
1. You ran `dotnet ef database drop` earlier
2. The database files were manually deleted
3. LocalDB instance was reset

The application code expects the database to exist, so when you try to register/login, it fails with "Cannot open database".

---

## ?? Understanding the Error Stack

```
SqlException: Cannot open database "AspNetAuth"
  ?
InvalidOperationException: transient failure
  ?
UserManager.CreateAsync() tries to connect to database
  ?
Database doesn't exist
  ?
Error!
```

---

## ?? Prevention

To avoid this in the future:

1. **Don't manually delete database files**
2. **Use EF Core commands** for database operations
3. **Check database exists** before running the app
4. **Keep migrations** - don't delete them

---

## ?? After Fix

Once the database is recreated, you can:

? Register new users  
? Login with credentials  
? All security features will work  
? Data will be encrypted (NRIC)  
? Audit logs will be recorded  

---

## ? Still Having Issues?

### Error: "Entity Framework tools not found"

Install EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

### Error: "No migrations found"

Check if migrations exist:
```bash
dotnet ef migrations list --context AuthDbContext
```

If empty, create a new migration:
```bash
dotnet ef migrations add InitialCreate --context AuthDbContext
dotnet ef database update --context AuthDbContext
```

### Error: "LocalDB not running"

Start LocalDB:
```bash
sqllocaldb start ProjectModels
```

Or create a new instance:
```bash
sqllocaldb create ProjectModels
sqllocaldb start ProjectModels
```

---

## ?? Expected Database Schema

After successful migration, your database should have:

### Tables Created:
- AspNetUsers (with custom fields)
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserTokens
- AspNetRoleClaims
- **AuditLogs** (custom table)

### Custom Fields in AspNetUsers:
- FirstName
- LastName
- Gender
- NRIC (encrypted)
- DateOfBirth
- ResumePath
- WhoAmI
- PasswordHistory
- LastPasswordChangeDate
- SessionToken
- SessionTokenExpiry
- TwoFactorCode
- TwoFactorCodeExpiry

---

**Status**: ? Fixes Applied  
**Next Step**: Stop app ? Run migration ? Restart app

---

**Need help?** Make sure to:
1. ? Stop the application FIRST
2. ? Run the migration command
3. ? Verify database created
4. ? Restart the application
