# 📊 COMPLETE SETUP FLOWCHART & CHECKLIST

---

## 🎯 SETUP FLOW (Do this in order)

```
START
  │
  ├─→ [1] CREATE DATABASE (5 minutes)
  │   ├─ Option A: Run setup-database.bat ⭐ EASIEST
  │   ├─ Option B: Run setup-database.ps1
  │   └─ Option C: Run dotnet ef database update
  │
  ├─→ [2] VERIFY DATABASE (2 minutes)
  │   ├─ Open SQL Server Management Studio
  │   ├─ Check Databases → SalesQuotationDb ✓
  │   └─ Expand and see 10 tables ✓
  │
  ├─→ [3] UPDATE CONFIGURATION (1 minute)
  │   ├─ Edit: SalesQuotation.API/appsettings.json
  │   └─ Change JwtSettings.SecretKey to 32+ char value
  │
  ├─→ [4] RUN APPLICATION (1 minute)
  │   ├─ cd SalesQuotation.API
  │   ├─ dotnet run
  │   └─ Wait for: "Now listening on: http://localhost:5000"
  │
  ├─→ [5] TEST APIs (5 minutes)
  │   ├─ Open browser: http://localhost:5000/swagger
  │   ├─ See all 51+ endpoints ✓
  │   └─ Try: POST /api/auth/login ✓
  │
  └─→ ✅ COMPLETE! Ready for mobile app integration
```

---

## ✅ DETAILED CHECKLIST

### **Phase 1: Database Setup** (5 minutes)

- [ ] Have SQL Server running
- [ ] Have SQL Management Studio open
- [ ] Navigate to project root directory
- [ ] Option A: Run `setup-database.bat` 
      OR
      Option B: Run `.\setup-database.ps1`
      OR
      Option C: Run `dotnet ef database update` in SalesQuotation.API folder
- [ ] Wait for completion (no errors)
- [ ] See "Database created successfully" message

### **Phase 2: Verification** (2 minutes)

- [ ] SQL Server Management Studio open
- [ ] Right-click Databases → Refresh
- [ ] Find: `SalesQuotationDb` in list
- [ ] Expand SalesQuotationDb → Tables
- [ ] Count tables (should be 10):
  - [ ] Users
  - [ ] Materials
  - [ ] Enquiries
  - [ ] Quotations
  - [ ] QuotationItems
  - [ ] Measurements
  - [ ] MeasurementCategories
  - [ ] EnquiryStatusConfigs
  - [ ] EnquiryProgress
  - [ ] FileUploads

### **Phase 3: Configuration** (1 minute)

- [ ] Open: `SalesQuotation.API/appsettings.json`
- [ ] Find: `"JwtSettings"` section
- [ ] Update: `"SecretKey"` to minimum 32 characters
  ```json
  "SecretKey": "your-super-secret-key-minimum-32-characters-long-please-change"
  ```
- [ ] Save file

### **Phase 4: Run Application** (1 minute)

- [ ] Open terminal/PowerShell
- [ ] Navigate to: `SalesQuotation.API`
- [ ] Run: `dotnet run`
- [ ] Wait for message: `Now listening on: http://localhost:5000`
- [ ] No errors in console

### **Phase 5: Test APIs** (5 minutes)

- [ ] Open browser: `http://localhost:5000/swagger`
- [ ] Page loads successfully
- [ ] See list of all 51+ endpoints
- [ ] See categories:
  - [ ] auth
  - [ ] staff
  - [ ] material
  - [ ] enquirystatusconfig
  - [ ] enquiryprogress
  - [ ] measurement
  - [ ] quotation
  - [ ] file
- [ ] Try to expand: `/api/auth/login`
- [ ] Click "Try it out"
- [ ] Send test request
- [ ] Get response (even if error, API is working)

### **Phase 6: Ready for Next** ✅

- [ ] Database created and verified
- [ ] Configuration updated
- [ ] Application running
- [ ] APIs responding
- [ ] Ready for mobile app integration
- [ ] Ready for user testing

---

## 📋 DATABASE SETUP METHODS COMPARISON

| Method | Time | Difficulty | Best For |
|--------|------|-----------|----------|
| **setup-database.bat** | 2 min | Very Easy | Windows users, first-timers |
| **setup-database.ps1** | 2 min | Very Easy | Windows PowerShell users |
| **dotnet ef update** | 3 min | Easy | Manual control, troubleshooting |
| **SQL Scripts** | 10 min | Medium | Advanced users, automation |

---

## 🔄 TROUBLESHOOTING FLOWCHART

```
Database Setup Failed?
│
├─→ Check SQL Server running
│   ├─ Open SQL Server Management Studio
│   ├─ Can you connect? YES → Go to next check
│   └─ Can you connect? NO → Start SQL Server service
│
├─→ Check connection string
│   ├─ Open appsettings.json
│   ├─ Verify Server name matches your SQL Server
│   ├─ Verify Database=SalesQuotationDb
│   └─ If using Express: (local)\SQLEXPRESS
│
├─→ Check EF Core tools
│   ├─ Run: dotnet tool list --global
│   ├─ See dotnet-ef? YES → Proceed
│   └─ See dotnet-ef? NO → Install: dotnet tool install --global dotnet-ef
│
├─→ Check migrations
│   ├─ Run: dotnet ef migrations list
│   ├─ See InitialCreate? YES → Run update
│   └─ See InitialCreate? NO → Create: dotnet ef migrations add InitialCreate
│
└─→ Clear and retry
    ├─ Delete database in SSMS
    ├─ Run: dotnet ef database update
    └─ Check for success message

Still failing? → See DATABASE_SETUP_GUIDE.md troubleshooting section
```

---

## ⏱️ TIME BREAKDOWN

```
Activity                        Time    Status
─────────────────────────────────────────────────
1. Create Database              5 min   ⏳ DO NOW
2. Verify in SSMS              2 min   ✓ Quick
3. Update JWT Secret           1 min   ✓ Quick
4. Run Application             1 min   ✓ Quick
5. Test Swagger UI             5 min   ✓ Quick
─────────────────────────────────────────────────
TOTAL                          15 min   🚀 Ready!
```

---

## 🎓 WHAT EACH PHASE DOES

### **Phase 1: Create Database**
Creates `SalesQuotationDb` with 10 tables, relationships, and indexes

### **Phase 2: Verify Database**
Confirms all tables were created correctly

### **Phase 3: Configure Settings**
Sets JWT secret key for security (change from default)

### **Phase 4: Run Application**
Starts ASP.NET Core API on http://localhost:5000

### **Phase 5: Test APIs**
Verifies all 51+ endpoints are working correctly

---

## 💾 FILES YOU'LL MODIFY

```
📁 SalesQuotation/
│
├─ 📝 appsettings.json ← MODIFY THIS
│   └─ Update "JwtSettings.SecretKey"
│
├─ 📂 SalesQuotation.API/
│   └─ (no modifications needed)
│
└─ 📂 wwwroot/
    ├─ uploads/  ← Creates automatically
    └─ pdfs/     ← Creates automatically
```

---

## 🔐 SECURITY NOTES

### JWT Secret Key
- **Requirement**: Minimum 32 characters
- **Current**: Empty (will cause error)
- **Action**: Update before running app
- **Example**: 
  ```
  "your-super-secret-key-that-is-at-least-32-characters-long-please"
  ```

### Database Connection
- **Uses**: Trusted Connection (Windows Authentication)
- **Alternative**: Can use SQL authentication with username/password
- **Security**: Keep credentials out of version control

---

## 📊 DATABASE STRUCTURE SUMMARY

```
10 Tables Created:

┌─ Users (Admin & Staff)
│   └─ Roles: Admin (0), Staff (1)
│
├─ Materials (Product Catalog)
│
├─ Enquiries (Customer Requests)
│   ├─ Assigned to Staff (AssignedStaffId)
│   ├─ Created by User (CreatedById)
│   └─ Multiple per customer
│
├─ Quotations (Sales Quotes)
│   ├─ Per Enquiry
│   ├─ Multiple per Enquiry allowed
│   └─ Status: Draft, Sent, Accepted, etc.
│
├─ QuotationItems (Line Items)
│   ├─ Part of Quotation
│   ├─ References Material
│   └─ Quantity × UnitCost = LineTotal
│
├─ Measurements (Site Measurements)
│   ├─ Per Enquiry
│   ├─ Category-based
│   └─ Auto-calculated values
│
├─ MeasurementCategories
│   ├─ Area
│   ├─ Length
│   ├─ Custom...
│
├─ EnquiryStatusConfigs
│   ├─ Initiated
│   ├─ Site Visited
│   ├─ Quotation Sent
│   ├─ Closed
│   └─ Custom statuses
│
├─ EnquiryProgress (Workflow History)
│   ├─ Tracks status changes
│   ├─ Comments/Notes
│   └─ Timestamp per change
│
└─ FileUploads
    ├─ Site photos
    ├─ Documents
    ├─ Sketches
    └─ Size limited to 10 MB
```

---

## ✨ POST-SETUP ACTIONS

After all 5 phases complete, you can:

1. **Create Admin User**
   - Via API endpoint
   - Via SQL script

2. **Configure Materials**
   - Add your product catalog
   - Set base costs

3. **Setup Statuses**
   - Define your workflow
   - Set colors for UI

4. **Connect Mobile App**
   - Point Flutter app to: `http://localhost:5000`
   - Get JWT token from login
   - Start building!

---

## 🚀 SUCCESS CRITERIA

You've succeeded when:

- ✅ Database exists in SQL Server
- ✅ All 10 tables created
- ✅ Application runs without errors
- ✅ Swagger UI loads at `/swagger`
- ✅ Can see all 51+ endpoints
- ✅ Can make test requests
- ✅ Authorization is working

---

## 📞 NEED HELP?

| Issue | File |
|-------|------|
| How do I create the database? | QUICK_DATABASE_SETUP.md |
| Need detailed DB info? | DATABASE_SETUP_GUIDE.md |
| Database errors? | DATABASE_SETUP_GUIDE.md (Troubleshooting) |
| Application errors? | QUICK_START.md (Troubleshooting) |
| Need complete guide? | COMPLETE_PROJECT_SETUP_SUMMARY.md |

---

## ⏰ READY? START HERE

```
1. Choose your setup method:
   ├─ setup-database.bat (Windows, easiest)
   ├─ setup-database.ps1 (PowerShell)
   └─ dotnet ef database update (manual)

2. Follow the 5 phases above

3. That's it! You're done in 15 minutes
```

---

**Status**: ✅ Ready to Execute
**Difficulty**: 🟢 Very Easy
**Time**: ⏱️ 15 minutes total

**Let's go!** 🚀
