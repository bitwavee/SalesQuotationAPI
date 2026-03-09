# Prompt for Claude Opus 4 — Flutter App Update

Copy everything below this line and paste it into your VS Code Copilot chat with Claude Opus 4.

---

## Context

I have a Flutter mobile app ("Sales & Quotation") that needs to be updated to work with my actual .NET backend API. The Flutter app currently has wrong API endpoints, wrong field names, and many stubbed/TODO modules. I need you to fix all mismatches and implement the missing modules.

## Backend API — Complete Endpoint Reference

Base URL: `http://localhost:5132` (or `https://localhost:7256` for HTTPS)

All responses use this envelope:
```json
{ "success": true, "data": { ... } }           // success
{ "success": false, "error": "...", "code": "..." }  // error
```

Auth header on all requests (except login): `Authorization: Bearer <jwt_token>`

### 1. Auth (no token needed for login)
```
POST /api/auth/login
  Body: { "email": "...", "password": "..." }
  Response.data: { "user": { "id", "name", "email", "phone", "role", "isActive", "createdAt" }, "token": "jwt" }
  NOTE: role is PascalCase → "Admin" or "Staff" (NOT "ADMIN"/"STAFF")

POST /api/auth/logout
  Header: Authorization: Bearer <token>
  Response.data: { "message": "Logged out successfully" }
```

### 2. Enquiry (unified endpoint, role-based filtering)
```
GET    /api/enquiry              → Admin gets ALL, Staff gets own assigned only
GET    /api/enquiry/{id}
POST   /api/enquiry              → Body: { "customerName", "customerEmail"?, "customerPhone", "customerAddress"?, "notes"? }
PUT    /api/enquiry/{id}         → Body: { "customerName"?, "customerEmail"?, "customerPhone"?, "customerAddress"?, "status"?, "notes"? }
DELETE /api/enquiry/{id}         → Admin only, soft delete
```
Enquiry response fields: `id, enquiryNumber, customerName, customerEmail, customerPhone, customerAddress, assignedStaffId, assignedStaff, status, notes, measurementsCount, quotationsCount, createdAt, updatedAt`

### 3. Enquiry Progress
```
GET  /api/enquiryprogress/enquiry/{enquiryId}           → Get history
POST /api/enquiryprogress/enquiry/{enquiryId}           → Body: { "status": "SITE_VISITED", "notes": "..." }
POST /api/enquiryprogress/enquiry/{enquiryId}/update-status → Same body, also updates enquiry status
```
Progress fields: `id, enquiryId, status, notes, updatedBy, createdAt`

### 4. Enquiry Status Config (Admin only for CUD)
```
GET    /api/enquirystatusconfig
GET    /api/enquirystatusconfig/{id}
POST   /api/enquirystatusconfig     → Body: { "statusName", "statusKey", "displayOrder", "color"? }
PUT    /api/enquirystatusconfig/{id} → Body: { "statusName"?, "statusKey"?, "displayOrder"?, "color"?, "isActive"? }
DELETE /api/enquirystatusconfig/{id}
```
Config fields: `id, statusName, statusKey, displayOrder, color, isActive, createdAt`

### 5. Staff Management (Admin only)
```
GET    /api/staff
GET    /api/staff/{id}
POST   /api/staff                → Body: { "name", "email", "phone"?, "password" }
PUT    /api/staff/{id}           → Body: { "name"?, "phone"?, "isActive"? }
DELETE /api/staff/{id}           → Soft delete
POST   /api/staff/assign-enquiry → Body: { "enquiryId": "guid", "staffId": "guid" }
PUT    /api/staff/{id}/role      → Body: { "role": "Admin" or "Staff" } (feature-flagged)
```
Staff response uses same UserDto: `id, name, email, phone, role, isActive, createdAt`

### 6. Material Management (Admin only for CUD)
```
GET    /api/material
GET    /api/material/{id}
POST   /api/material             → Body: { "name", "description"?, "unit", "baseCost" }
PUT    /api/material/{id}        → Body: { "name"?, "description"?, "unit"?, "baseCost"?, "isActive"? }
DELETE /api/material/{id}
```
Material fields: `id, name, description, unit, baseCost, isActive, createdAt`

### 7. Measurement
```
GET    /api/measurement/enquiry/{enquiryId}    → List for enquiry
GET    /api/measurement/{id}
POST   /api/measurement/{enquiryId}            → Body: { "categoryId": "guid", "measurementData": { "length": 3.5, "width": 2.0 }, "notes"? }
PUT    /api/measurement/{id}                   → Body: { "categoryId"?, "measurementData"?: { "length": 4.0, "width": 2.5 }, "notes"? }
DELETE /api/measurement/{id}
POST   /api/measurement/convert/meter-to-sqft  → Body: { "length": 3.5, "breadth": 2.0 }
```
Measurement fields: `id, enquiryId, categoryId, category { id, categoryName, categoryKey, measurementFields }, measurementData (JSON string), calculatedValue, notes, createdAt`
Categories seeded: Area (length×width), Length (value), Volume (length×width×height)

### 8. Quotation
```
GET    /api/quotation/enquiry/{enquiryId}
GET    /api/quotation/{id}
POST   /api/quotation                          → Body: { "enquiryId": "guid", "quotationNumber", "taxPercentage", "notes"? }
PUT    /api/quotation/{id}                     → Body: { "quotationNumber"?, "taxPercentage"?, "notes"?, "status"? }
DELETE /api/quotation/{id}
GET    /api/quotation/{id}/pdf                 → Returns PDF bytes (application/pdf)
GET    /api/quotation/{id}/download-pdf        → Returns saved PDF file
POST   /api/quotation/{id}/send                → Marks quotation as sent
```
Quotation fields: `id, enquiryId, quotationNumber, quotationDate, validUntil, subtotal, taxPercentage, taxAmount, totalAmount, notes, status, pdfPath, items[], sentAt, createdAt`
QuotationItem fields: `id, materialName, quantity, unitCost, lineTotal, notes`

### 9. File Upload
```
POST   /api/file/upload/{enquiryId}?category=SITE_PHOTO  → multipart/form-data, field name "file"
GET    /api/file/enquiry/{enquiryId}                      → List files for enquiry
GET    /api/file/{id}                                     → File metadata
GET    /api/file/download/{id}                            → Download actual file
DELETE /api/file/{id}
```
File fields: `id, enquiryId, fileName, fileType, fileSize, filePath, category, uploadedAt`

---

## CHANGES REQUIRED IN FLUTTER APP

### Phase 1: Fix existing code (breaking mismatches)

1. **Change API base URL** from `localhost:5000` to `http://10.0.2.2:5132` (for Android emulator) or `http://localhost:5132` (for physical device / iOS). Make this configurable.

2. **Fix role comparison** — backend sends `"Admin"` and `"Staff"` (PascalCase), NOT `"ADMIN"` / `"STAFF"`. Update all role checks to be case-insensitive: `user.role.toUpperCase() == "ADMIN"` or normalize on parse.

3. **Fix Enquiry API paths:**
   - Change `GET /api/staff/enquiries` → `GET /api/enquiry`
   - Change `POST /api/staff/enquiries` → `POST /api/enquiry`
   - Change `GET /api/staff/enquiries/:id` → `GET /api/enquiry/:id`
   - Change `GET /api/admin/enquiries` → `GET /api/enquiry` (same endpoint — backend filters by role automatically)
   - Remove separate admin/staff enquiry API classes — use ONE enquiry service

4. **Fix status update:**
   - Change `PUT /api/staff/enquiries/:id/status` → `POST /api/enquiryprogress/enquiry/:id/update-status`
   - Change request body from `{ "new_status": "...", "comment": "..." }` → `{ "status": "...", "notes": "..." }`

5. **Fix Enquiry model** — ensure it handles these exact camelCase field names from the API: `id, enquiryNumber, customerName, customerEmail, customerPhone, customerAddress, assignedStaffId, assignedStaff, status, notes, measurementsCount, quotationsCount, createdAt, updatedAt`. Remove any snake_case fallbacks that don't match.

### Phase 2: Implement stubbed modules

6. **Staff Management screen (Admin):**
   - List all staff: `GET /api/staff`
   - Create staff form: `POST /api/staff` with fields: name, email, phone, password
   - Edit staff: `PUT /api/staff/{id}` with fields: name, phone, isActive toggle
   - Delete staff: `DELETE /api/staff/{id}`
   - Assign enquiry to staff: `POST /api/staff/assign-enquiry` with enquiryId + staffId dropdowns

7. **Material Management screen (Admin):**
   - List materials: `GET /api/material`
   - Create material form: `POST /api/material` with fields: name, description, unit (dropdown: sqft, meter, piece, kg), baseCost
   - Edit material: `PUT /api/material/{id}`
   - Delete material: `DELETE /api/material/{id}`

8. **Measurement module (on Enquiry Detail screen):**
   - List measurements for enquiry: `GET /api/measurement/enquiry/{enquiryId}`
   - Add measurement form: `POST /api/measurement/{enquiryId}` — show category dropdown (Area/Length/Volume), dynamic fields based on category, calculated value preview
   - Edit measurement: `PUT /api/measurement/{id}`
   - Delete measurement: `DELETE /api/measurement/{id}`
   - Unit conversion helper: `POST /api/measurement/convert/meter-to-sqft`

9. **Quotation module (on Enquiry Detail screen):**
   - List quotations: `GET /api/quotation/enquiry/{enquiryId}`
   - Create quotation: `POST /api/quotation` with enquiryId, quotationNumber (auto-generate), taxPercentage
   - Edit quotation: `PUT /api/quotation/{id}`
   - View/Download PDF: `GET /api/quotation/{id}/pdf` — display in-app or save
   - Send quotation: `POST /api/quotation/{id}/send`
   - Delete quotation: `DELETE /api/quotation/{id}`

10. **Enquiry Progress/History (on Enquiry Detail screen):**
    - Show timeline: `GET /api/enquiryprogress/enquiry/{enquiryId}`
    - Add comment/status change: `POST /api/enquiryprogress/enquiry/{enquiryId}`
    - Status dropdown should be populated from: `GET /api/enquirystatusconfig`

11. **File Upload (on Enquiry Detail screen):**
    - Upload photo/document: `POST /api/file/upload/{enquiryId}?category=SITE_PHOTO` using multipart form
    - List files: `GET /api/file/enquiry/{enquiryId}`
    - Download: `GET /api/file/download/{id}`
    - Delete: `DELETE /api/file/{id}`
    - Use the existing `image_picker` and `camera` dependencies

12. **Enquiry Status Config screen (Admin):**
    - CRUD for status workflow: `GET/POST/PUT/DELETE /api/enquirystatusconfig`

### Phase 3: Architecture improvements

13. Create a centralized `ApiService` class with:
    - Base URL configuration (dev/prod)
    - Automatic token injection from SharedPreferences
    - Standard error handling that parses `{ "success": false, "error": "...", "code": "..." }`
    - HTTP timeout: 30 seconds

14. Create these service classes following the Provider pattern:
    - `AuthProvider` (exists — fix it)
    - `EnquiryProvider` (exists — fix API paths)
    - `StaffProvider` (new)
    - `MaterialProvider` (new)
    - `MeasurementProvider` (new)
    - `QuotationProvider` (new)
    - `FileProvider` (new)
    - `EnquiryStatusConfigProvider` (new)

15. Create/update these model classes (all fields camelCase, fromJson/toJson):
    - `User` (exists — fix role casing)
    - `Enquiry` (exists — verify fields)
    - `EnquiryProgress` (new)
    - `EnquiryStatusConfig` (new)
    - `Staff` = same as User (reuse UserDto)
    - `Material` (new)
    - `Measurement` (new)
    - `MeasurementCategory` (new)
    - `Quotation` (new)
    - `QuotationItem` (new)
    - `FileUpload` (new)

### Implementation order
Start with Phase 1 (fixes) → then Phase 2 items 6-7 (admin screens) → then 8-11 (enquiry detail sub-modules) → then Phase 3.

Please implement all changes. Scan my current Flutter project structure first, then make the changes file by file.
