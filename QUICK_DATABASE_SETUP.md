# ⚡ QUICK DATABASE SETUP - 5 MINUTES

**Status**: Ready to Create Database ✅

---

## 🎯 FASTEST WAY (2 Options)

### **Option A: Use Automated Script** ⭐ EASIEST

#### **Windows (Command Prompt)**
```batch
setup-database.bat
```

#### **Windows (PowerShell)**
```powershell
.\setup-database.ps1
```

**That's it!** The script will:
1. ✅ Install .NET EF tools
2. ✅ Create migrations
3. ✅ Create database in SQL Server
4. ✅ Create all 10 tables
5. ✅ Create all indexes

---

### **Option B: Manual Commands** (If script doesn't work)

**Step 1: Open Terminal/PowerShell**
```bash
# Navigate to project root
cd C:\Path\To\SalesQuotationProjects
```

**Step 2: Run these commands**
```bash
# Install EF tools (one time only)
dotnet tool install --global dotnet-ef

# Navigate to API project
cd SalesQuotation.API

# Create database
dotnet ef database update
```

**Done!** Your database is created.

---

## ✅ VERIFY DATABASE CREATED

### **In SQL Server Management Studio (SSMS)**

1. **Right-click Databases** → **Refresh**
2. **Look for**: `SalesQuotationDb` ✓
3. **Expand it** to see these 10 tables:
   - Users ✓
   - Materials ✓
   - Enquiries ✓
   - Quotations ✓
   - QuotationItems ✓
   - Measurements ✓
   - MeasurementCategories ✓
   - EnquiryStatusConfigs ✓
   - EnquiryProgress ✓
   - FileUploads ✓

### **Or run this SQL query in SSMS**
```sql
USE SalesQuotationDb
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' ORDER BY TABLE_NAME;
```

**You should see 10 rows** ✓

---

## 🔧 CONFIGURE & RUN

### **1. Update JWT Secret** (IMPORTANT)

Edit: `SalesQuotation.API/appsettings.json`

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-minimum-32-characters-long-please-change-this-for-production",
    "ExpiryMinutes": 10080
  }
}
```

### **2. Run Application**

```bash
cd SalesQuotation.API
dotnet run
```

**You should see:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### **3. Test APIs**

Open browser: **http://localhost:5000/swagger**

You should see all 51+ API endpoints! 🎉

---

## 📋 TROUBLESHOOTING

| Problem | Solution |
|---------|----------|
| **"dotnet-ef not found"** | Run: `dotnet tool install --global dotnet-ef` |
| **"Connection string error"** | Check `appsettings.json` connection string syntax |
| **"Database already exists"** | Drop it in SSMS, run migration again |
| **"SQL Server not responding"** | Ensure SQL Server service is running |
| **"Access denied"** | Check if Trusted_Connection is enabled in connection string |

---

## 🗂️ CONNECTION STRINGS

### **For SQL Express (Default)**
```
Server=(local)\SQLEXPRESS;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;
```

### **For SQL Server 2022**
```
Server=localhost;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;
```

### **For SQL Server with User/Password**
```
Server=localhost;Database=SalesQuotationDb;User Id=sa;Password=YourPassword;Encrypt=False;TrustServerCertificate=True;
```

---

## 📊 DATABASE STRUCTURE

```
SalesQuotationDb/
├── Users (Admin/Staff users)
├── Enquiries (Customer requests)
├── Quotations (Sales quotes)
├── QuotationItems (Line items)
├── Materials (Product catalog)
├── Measurements (Site measurements)
├── MeasurementCategories (Measurement types)
├── EnquiryStatusConfigs (Status definitions)
├── EnquiryProgress (Workflow history)
└── FileUploads (Documents/photos)
```

---

## ✨ NEXT STEPS

- [ ] Run setup script OR manual commands
- [ ] Verify database in SSMS
- [ ] Update JWT secret
- [ ] Run `dotnet run`
- [ ] Test at http://localhost:5000/swagger
- [ ] Create admin user (optional, via API)

---

## 📖 DETAILED GUIDE

For more information, see: **DATABASE_SETUP_GUIDE.md**

---

**Status**: Ready to Execute ✅
**Time Required**: 5 minutes ⏱️
**Difficulty**: Very Easy 🟢

---

**Run the setup now!** 🚀
