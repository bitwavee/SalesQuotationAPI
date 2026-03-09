# 🗄️ DATABASE SETUP GUIDE - SQL Server

## ✅ Status: Ready to Create Database

You have:
- ✅ SQL Server installed
- ✅ SQL Server Management Studio 22 connected
- ✅ .NET project with Entity Framework Core
- ✅ Connection string configured

---

## 📋 SETUP OPTIONS

### **Option 1: Use Entity Framework Core (RECOMMENDED)**
### **Option 2: Manual SQL Scripts**

Choose the method that works best for you.

---

## 🔧 OPTION 1: Entity Framework Core (RECOMMENDED)

### **Step 1: Configure Connection String**

Edit `SalesQuotation.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-must-be-at-least-32-characters-long-change-this-please",
    "ExpiryMinutes": 10080
  }
}
```

**Connection String Variants:**

```json
// For SQL Express (default)
"Server=(local)\\SQLEXPRESS;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"

// For SQL Server 2019/2022
"Server=localhost;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"

// For SQL Server with username/password
"Server=localhost;Database=SalesQuotationDb;User Id=sa;Password=YourPassword;Encrypt=False;TrustServerCertificate=True;"

// For TCP/IP connection
"Server=127.0.0.1;Database=SalesQuotationDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
```

### **Step 2: Open Terminal in Project Directory**

```bash
cd SalesQuotation.API
```

### **Step 3: Check for Existing Migrations**

```bash
dotnet ef migrations list
```

### **Step 4: Create Initial Migration (if none exists)**

```bash
dotnet ef migrations add InitialCreate --project ../SalesQuotation.Infrastructure
```

**Output should look like:**
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

### **Step 5: Create Database**

```bash
dotnet ef database update
```

**Expected Output:**
```
Build started...
Build succeeded.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (50ms) [Parameters=]
      CREATE DATABASE [SalesQuotationDb];
...
Done.
```

### **Step 6: Verify Database Created**

In SQL Server Management Studio (SSMS):
1. Right-click **Databases** → **Refresh**
2. You should see **SalesQuotationDb** in the list
3. Expand it to see all tables:
   - Users
   - Materials
   - Enquiries
   - Quotations
   - QuotationItems
   - Measurements
   - MeasurementCategories
   - EnquiryStatusConfigs
   - EnquiryProgress
   - FileUploads

---

## 📊 OPTION 2: Manual SQL Scripts

If EF Core method doesn't work, use these manual SQL scripts in SSMS.

### **Step 1: Create Database**

In SSMS, run:

```sql
CREATE DATABASE [SalesQuotationDb]
GO

USE [SalesQuotationDb]
GO
```

### **Step 2: Create All Tables**

```sql
-- Users Table
CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Phone] nvarchar(max),
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] int NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
)
GO

-- Materials Table
CREATE TABLE [Materials] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max),
    [Unit] nvarchar(max) NOT NULL,
    [BaseCost] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedById] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Materials] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Materials_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
)
GO

-- MeasurementCategories Table
CREATE TABLE [MeasurementCategories] (
    [Id] uniqueidentifier NOT NULL,
    [CategoryName] nvarchar(max) NOT NULL,
    [CategoryKey] nvarchar(max) NOT NULL,
    [MeasurementFields] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_MeasurementCategories] PRIMARY KEY ([Id])
)
GO

-- EnquiryStatusConfigs Table
CREATE TABLE [EnquiryStatusConfigs] (
    [Id] uniqueidentifier NOT NULL,
    [StatusName] nvarchar(max) NOT NULL,
    [StatusValue] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [ColorHex] nvarchar(max),
    [IsActive] bit NOT NULL,
    [CreatedById] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EnquiryStatusConfigs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EnquiryStatusConfigs_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
)
GO

-- Enquiries Table
CREATE TABLE [Enquiries] (
    [Id] uniqueidentifier NOT NULL,
    [EnquiryNumber] nvarchar(max) NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerEmail] nvarchar(max),
    [CustomerPhone] nvarchar(max) NOT NULL,
    [CustomerAddress] nvarchar(max),
    [AssignedStaffId] uniqueidentifier,
    [Status] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max),
    [IsDeleted] bit NOT NULL,
    [CreatedById] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [DeletedAt] datetime2,
    CONSTRAINT [PK_Enquiries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enquiries_Users_AssignedStaffId] FOREIGN KEY ([AssignedStaffId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Enquiries_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
)
GO

-- Measurements Table
CREATE TABLE [Measurements] (
    [Id] uniqueidentifier NOT NULL,
    [EnquiryId] uniqueidentifier NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [MeasurementData] nvarchar(max) NOT NULL,
    [CalculatedValue] decimal(18,2) NOT NULL,
    [Notes] nvarchar(max),
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Measurements] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Measurements_MeasurementCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [MeasurementCategories]([Id]),
    CONSTRAINT [FK_Measurements_Enquiries_EnquiryId] FOREIGN KEY ([EnquiryId]) REFERENCES [Enquiries]([Id])
)
GO

-- Quotations Table
CREATE TABLE [Quotations] (
    [Id] uniqueidentifier NOT NULL,
    [EnquiryId] uniqueidentifier NOT NULL,
    [QuotationNumber] nvarchar(max) NOT NULL,
    [QuotationDate] datetime2 NOT NULL,
    [ValidUntil] datetime2,
    [Subtotal] decimal(18,2) NOT NULL,
    [TaxPercentage] decimal(18,2) NOT NULL,
    [TaxAmount] decimal(18,2),
    [TotalAmount] decimal(18,2) NOT NULL,
    [Notes] nvarchar(max),
    [Status] nvarchar(max) NOT NULL,
    [PdfPath] nvarchar(max),
    [CreatedById] uniqueidentifier NOT NULL,
    [SentAt] datetime2,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Quotations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Quotations_Enquiries_EnquiryId] FOREIGN KEY ([EnquiryId]) REFERENCES [Enquiries]([Id]),
    CONSTRAINT [FK_Quotations_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
)
GO

-- QuotationItems Table
CREATE TABLE [QuotationItems] (
    [Id] uniqueidentifier NOT NULL,
    [QuotationId] uniqueidentifier NOT NULL,
    [MaterialId] uniqueidentifier NOT NULL,
    [Quantity] decimal(18,2) NOT NULL,
    [UnitCost] decimal(18,2) NOT NULL,
    [LineTotal] decimal(18,2) NOT NULL,
    [Notes] nvarchar(max),
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_QuotationItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuotationItems_Materials_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materials]([Id]),
    CONSTRAINT [FK_QuotationItems_Quotations_QuotationId] FOREIGN KEY ([QuotationId]) REFERENCES [Quotations]([Id])
)
GO

-- EnquiryProgress Table
CREATE TABLE [EnquiryProgress] (
    [Id] uniqueidentifier NOT NULL,
    [EnquiryId] uniqueidentifier NOT NULL,
    [OldStatus] nvarchar(max),
    [NewStatus] nvarchar(max) NOT NULL,
    [Comment] nvarchar(max),
    [CreatedById] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EnquiryProgress] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EnquiryProgress_Enquiries_EnquiryId] FOREIGN KEY ([EnquiryId]) REFERENCES [Enquiries]([Id]),
    CONSTRAINT [FK_EnquiryProgress_Users_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [Users]([Id])
)
GO

-- FileUploads Table
CREATE TABLE [FileUploads] (
    [Id] uniqueidentifier NOT NULL,
    [EnquiryId] uniqueidentifier,
    [QuotationId] uniqueidentifier,
    [FileName] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [FileType] nvarchar(max),
    [FileSizeBytes] int,
    [UploadedById] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FileUploads] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FileUploads_Enquiries_EnquiryId] FOREIGN KEY ([EnquiryId]) REFERENCES [Enquiries]([Id]),
    CONSTRAINT [FK_FileUploads_Quotations_QuotationId] FOREIGN KEY ([QuotationId]) REFERENCES [Quotations]([Id]),
    CONSTRAINT [FK_FileUploads_Users_UploadedById] FOREIGN KEY ([UploadedById]) REFERENCES [Users]([Id])
)
GO

-- Create Indexes
CREATE UNIQUE INDEX [IX_Users_Email] ON [Users]([Email])
GO

CREATE INDEX [IX_Materials_CreatedById] ON [Materials]([CreatedById])
GO

CREATE INDEX [IX_EnquiryStatusConfigs_CreatedById] ON [EnquiryStatusConfigs]([CreatedById])
GO

CREATE INDEX [IX_Enquiries_AssignedStaffId] ON [Enquiries]([AssignedStaffId])
GO

CREATE INDEX [IX_Enquiries_CreatedById] ON [Enquiries]([CreatedById])
GO

CREATE INDEX [IX_Measurements_CategoryId] ON [Measurements]([CategoryId])
GO

CREATE INDEX [IX_Measurements_EnquiryId] ON [Measurements]([EnquiryId])
GO

CREATE INDEX [IX_Quotations_CreatedById] ON [Quotations]([CreatedById])
GO

CREATE INDEX [IX_Quotations_EnquiryId] ON [Quotations]([EnquiryId])
GO

CREATE INDEX [IX_QuotationItems_MaterialId] ON [QuotationItems]([MaterialId])
GO

CREATE INDEX [IX_QuotationItems_QuotationId] ON [QuotationItems]([QuotationId])
GO

CREATE INDEX [IX_EnquiryProgress_CreatedById] ON [EnquiryProgress]([CreatedById])
GO

CREATE INDEX [IX_EnquiryProgress_EnquiryId] ON [EnquiryProgress]([EnquiryId])
GO

CREATE INDEX [IX_FileUploads_EnquiryId] ON [FileUploads]([EnquiryId])
GO

CREATE INDEX [IX_FileUploads_QuotationId] ON [FileUploads]([QuotationId])
GO

CREATE INDEX [IX_FileUploads_UploadedById] ON [FileUploads]([UploadedById])
GO

-- Verify tables created
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' ORDER BY TABLE_NAME;
```

---

## 📝 STEP-BY-STEP INSTRUCTIONS

### **Using Entity Framework Core (RECOMMENDED)**

#### **1. Open Command Prompt/PowerShell**
```bash
# Navigate to project directory
cd C:\Path\To\SalesQuotationProjects\SalesQuotation.API

# Or if using PowerShell, navigate to directory first
```

#### **2. Ensure EF Core tools are installed**
```bash
dotnet tool install --global dotnet-ef
```

#### **3. Create Initial Migration** (if needed)
```bash
dotnet ef migrations add InitialCreate --project ../SalesQuotation.Infrastructure
```

#### **4. Create Database**
```bash
dotnet ef database update
```

#### **5. Verify in SSMS**
1. Open SQL Server Management Studio
2. Expand **Databases**
3. Right-click → **Refresh**
4. You should see **SalesQuotationDb**

---

## ✅ VERIFICATION STEPS

### **Check Database Created**

In SSMS, run this query:

```sql
-- Check if database exists
SELECT name FROM sys.databases WHERE name = 'SalesQuotationDb';

-- Check all tables
USE SalesQuotationDb;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' ORDER BY TABLE_NAME;

-- Count tables (should be 10)
SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo';
```

**Expected Result:**
```
TableCount: 10

Tables:
1. Users
2. Materials
3. MeasurementCategories
4. EnquiryStatusConfigs
5. Enquiries
6. Measurements
7. Quotations
8. QuotationItems
9. EnquiryProgress
10. FileUploads
```

---

## 🌱 SEED DATA (Optional - Create Admin User)

After database is created, run this script in SSMS to create an admin user:

```sql
USE [SalesQuotationDb]
GO

-- Insert Admin User
INSERT INTO [Users] (
    [Id], 
    [Name], 
    [Email], 
    [Phone], 
    [PasswordHash], 
    [Role], 
    [IsActive], 
    [IsDeleted], 
    [CreatedAt], 
    [UpdatedAt]
) VALUES (
    NEWID(),
    'Administrator',
    'admin@salesquotation.com',
    '1234567890',
    'YWRtaW5wYXNzMTIz', -- Base64 encoded dummy hash (use proper hashing in production)
    0,  -- 0 = Admin
    1,
    0,
    GETUTCDATE(),
    GETUTCDATE()
)
GO

-- Verify insert
SELECT * FROM [Users]
GO
```

---

## 🔐 Update JWT Secret Key

Before running the application, update the JWT secret in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-at-least-32-characters-long-please-change-this",
    "ExpiryMinutes": 10080
  }
}
```

**Generate a strong secret key:**
```bash
# PowerShell
-join ([1..32] | ForEach-Object { [char][System.Security.Cryptography.RandomNumberGenerator]::GetBytes(1)[0] % 93 + 33 })
```

Or use: `GenerateSecretKeyUtil` online tool

---

## 🚀 NEXT STEPS

1. ✅ Create database (using EF Core or manual SQL)
2. ✅ Verify tables in SSMS
3. ✅ Update `appsettings.json` with JWT secret
4. ✅ Run the application: `dotnet run`
5. ✅ Test APIs: `http://localhost:5000/swagger`
6. ✅ Create admin user through API or SSMS

---

## 🆘 TROUBLESHOOTING

### **Problem: "Connection string not found"**
**Solution**: Ensure `appsettings.json` has correct connection string

### **Problem: "Login failed for user"**
**Solution**: Check SQL Server authentication is enabled or use `sa` account

### **Problem: "Database already exists"**
**Solution**: 
```sql
DROP DATABASE [SalesQuotationDb];
```
Then run migration again

### **Problem: Migration error**
**Solution**:
```bash
# Remove last migration
dotnet ef migrations remove

# Try again
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **Problem: Can't connect to SQL Server**
**Solution**: 
- Verify SQL Server is running
- Check SQL Server name (localhost vs SQLEXPRESS)
- Enable TCP/IP protocol
- Restart SQL Server service

---

## 📊 Database Diagram

```
Users (Admin/Staff)
├── Materials (Foreign Key)
├── Enquiries (Foreign Key - Created/Assigned)
│   ├── Measurements (Foreign Key)
│   ├── Quotations (Foreign Key)
│   │   └── QuotationItems (Foreign Key)
│   ├── EnquiryProgress (Foreign Key)
│   └── FileUploads (Foreign Key)
├── EnquiryStatusConfigs (Foreign Key - Created)
└── EnquiryProgress (Foreign Key - Updated)
```

---

## ✨ Status Check

After setup, verify everything is working:

```bash
# 1. Database created ✅
# 2. Tables created ✅
# 3. Connections working ✅
# 4. Ready for API ✅
```

---

**Created**: 2025-03-04
**Version**: 1.0
**Status**: Ready to Execute ✅

Choose **Option 1 (EF Core)** or **Option 2 (Manual SQL)** and follow the steps above.
