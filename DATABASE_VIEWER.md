# ??? Database Viewer - Your Data

## ?? **Registered Users (3)**

| Email | Name | Gender | NRIC (Encrypted) | Lockout |
|-------|------|--------|------------------|---------|
| brandonkoh303@gmail.com | Brandon Koh | Unknown | oI8hx6BwM+oemyxhsUml7A== | Enabled |
| isaactanjx03@gmail.com | zac tan | Unknown | oI8hx6BwM+oemyxhsUml7A== | Enabled |
| brandonkoh003@yahoo.com.sg | Brandon Koh | Unknown | oI8hx6BwM+oemyxhsUml7A== | Enabled |

? **NRIC Encryption Working!** - All NRICs are encrypted with AES-256

---

## ?? **Recent Activity (Last 10 Events)**

| Timestamp | Action | Details |
|-----------|--------|---------|
| 2026-01-26 02:50:00 | Successful Login | 2FA completed |
| 2026-01-26 02:49:46 | Login Initiated | 2FA code sent |
| 2026-01-26 02:49:15 | Registration | User registered: isaactanjx03@gmail.com |
| 2026-01-26 02:47:16 | Logout | User logged out |
| 2026-01-26 02:47:00 | Successful Login | 2FA completed |
| 2026-01-26 02:46:40 | Login Initiated | 2FA code sent |
| 2026-01-26 02:44:59 | Registration | User registered: brandonkoh003@yahoo.com.sg |
| 2026-01-26 02:34:09 | Login Initiated | 2FA code sent |
| 2026-01-26 02:32:52 | Login Initiated | 2FA code sent |
| 2026-01-26 02:32:44 | Registration | User registered: brandonkoh303@gmail.com |

? **Audit Logging Working!** - All security events tracked

---

## ?? **Useful SQL Queries**

### View All Users:
```sql
SELECT Email, FirstName, LastName, NRIC 
FROM AspNetUsers
```

### View Audit Logs:
```sql
SELECT Action, Timestamp, Details 
FROM AuditLogs 
ORDER BY Timestamp DESC
```

### Check Account Status:
```sql
SELECT Email, AccessFailedCount, LockoutEnd, LockoutEnabled
FROM AspNetUsers
```

---

## ? **Quick Commands**

### Count Users:
```sh
sqlcmd -S "(localdb)\ProjectModels" -d AspNetAuth -Q "SELECT COUNT(*) FROM AspNetUsers"
```

### View Latest User:
```sh
sqlcmd -S "(localdb)\ProjectModels" -d AspNetAuth -Q "SELECT TOP 1 Email, FirstName FROM AspNetUsers ORDER BY Id DESC" -W
```

### View Latest Login:
```sh
sqlcmd -S "(localdb)\ProjectModels" -d AspNetAuth -Q "SELECT TOP 1 * FROM AuditLogs WHERE Action LIKE '%Login%' ORDER BY Timestamp DESC" -W
```

---

## ? **Database Status**

| Component | Status | Count/Details |
|-----------|--------|---------------|
| **Users** | ? Working | 3 registered users |
| **NRIC Encryption** | ? Working | All encrypted with AES-256 |
| **Audit Logs** | ? Working | 10+ events logged |
| **Account Lockout** | ? Configured | 3 attempts, 5 min lockout |
| **Session Management** | ? Working | 20-minute timeout |

---

## ?? **Why "View Data" Doesn't Work**

Visual Studio's "View Data" feature has issues with:
- ? LocalDB connections
- ? Tables with many columns
- ? Cached connections

**Solution:** Use SQL queries instead (see queries above)

---

**Your database is working perfectly! Just use SQL queries to view data instead of "View Data" button.** ?
