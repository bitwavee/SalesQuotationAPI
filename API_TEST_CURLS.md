###############################################################################
# Sales Quotation API – cURL Test Script (38 endpoints)
# Base URL : https://localhost:7256  (or http://localhost:5132)
#
# HOW TO USE
#   1. Run step 1 first to get a TOKEN.
#   2. Replace every {{TOKEN}} with the JWT you received.
#   3. Run steps in order – later calls need IDs from earlier responses.
#   4. Replace {{…}} placeholders with real IDs from the responses.
###############################################################################

# ─────────────────────────────────────────────────────────────────────────────
# 1. AUTH
# ─────────────────────────────────────────────────────────────────────────────

## 1.1 Login (Admin)  ← run this FIRST, copy the "token" value
curl -k -X POST https://localhost:7256/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"admin@salesquotation.com\",\"password\":\"Admin@123\"}"

## 1.2 Logout
curl -k -X POST https://localhost:7256/api/auth/logout ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 2. STAFF  (Admin only – 7 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 2.1 Get all staff
curl -k -X GET https://localhost:7256/api/staff ^
  -H "Authorization: Bearer {{TOKEN}}"

## 2.2 Create staff  → copy returned "id" as {{STAFF_ID}}
curl -k -X POST https://localhost:7256/api/staff ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"name\":\"John Field\",\"email\":\"john@salesquotation.com\",\"phone\":\"9876543210\",\"password\":\"Staff@123\"}"

## 2.3 Get staff by ID
curl -k -X GET https://localhost:7256/api/staff/{{STAFF_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 2.4 Update staff
curl -k -X PUT https://localhost:7256/api/staff/{{STAFF_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"name\":\"John Field Updated\",\"phone\":\"9876500000\",\"isActive\":true}"

## 2.5 Change role (needs Features:EnableRoleManagement = true)
curl -k -X PUT https://localhost:7256/api/staff/{{STAFF_ID}}/role ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"role\":\"Admin\"}"

## 2.6 Assign enquiry to staff (run AFTER creating enquiry in step 5)
curl -k -X POST https://localhost:7256/api/staff/assign-enquiry ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"enquiryId\":\"{{ENQUIRY_ID}}\",\"staffId\":\"{{STAFF_ID}}\"}"

## 2.7 Delete staff (soft delete)
curl -k -X DELETE https://localhost:7256/api/staff/{{STAFF_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 3. MATERIAL  (Admin only – 5 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 3.1 Get all materials
curl -k -X GET https://localhost:7256/api/material ^
  -H "Authorization: Bearer {{TOKEN}}"

## 3.2 Create material  → copy returned "id" as {{MATERIAL_ID}}
curl -k -X POST https://localhost:7256/api/material ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"name\":\"Granite Slab 20mm\",\"description\":\"Premium black granite\",\"unit\":\"sqft\",\"baseCost\":150.00}"

## 3.3 Get material by ID
curl -k -X GET https://localhost:7256/api/material/{{MATERIAL_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 3.4 Update material
curl -k -X PUT https://localhost:7256/api/material/{{MATERIAL_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"name\":\"Granite Slab 20mm - Premium\",\"baseCost\":175.50,\"isActive\":true}"

## 3.5 Delete material
curl -k -X DELETE https://localhost:7256/api/material/{{MATERIAL_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 4. ENQUIRY STATUS CONFIG  (Admin only – 5 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 4.1 Get all status configs (already seeded – 6 rows)  → copy any "id" as {{CONFIG_ID}}
curl -k -X GET https://localhost:7256/api/enquirystatusconfig ^
  -H "Authorization: Bearer {{TOKEN}}"

## 4.2 Create status config
curl -k -X POST https://localhost:7256/api/enquirystatusconfig ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"statusName\":\"On Hold\",\"statusKey\":\"ON_HOLD\",\"displayOrder\":7,\"color\":\"#FF9800\"}"

## 4.3 Get status config by ID
curl -k -X GET https://localhost:7256/api/enquirystatusconfig/{{CONFIG_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 4.4 Update status config
curl -k -X PUT https://localhost:7256/api/enquirystatusconfig/{{CONFIG_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"statusName\":\"Updated Status\",\"color\":\"#00BCD4\",\"isActive\":true}"

## 4.5 Delete status config
curl -k -X DELETE https://localhost:7256/api/enquirystatusconfig/{{CONFIG_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 5. ENQUIRY  (5 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 5.1 Get all enquiries
curl -k -X GET https://localhost:7256/api/enquiry ^
  -H "Authorization: Bearer {{TOKEN}}"

## 5.2 Create enquiry  → copy returned "id" as {{ENQUIRY_ID}}
curl -k -X POST https://localhost:7256/api/enquiry ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"customerName\":\"Rajesh Kumar\",\"customerEmail\":\"rajesh@example.com\",\"customerPhone\":\"9988776655\",\"customerAddress\":\"45 MG Road, Bangalore 560001\",\"notes\":\"Interested in granite countertops\"}"

## 5.3 Get enquiry by ID
curl -k -X GET https://localhost:7256/api/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 5.4 Update enquiry
curl -k -X PUT https://localhost:7256/api/enquiry/{{ENQUIRY_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"customerName\":\"Rajesh Kumar (Updated)\",\"status\":\"SITE_VISITED\",\"notes\":\"Site visited on 10th March\"}"

## 5.5 Delete enquiry (Admin only, soft delete)
curl -k -X DELETE https://localhost:7256/api/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 6. ENQUIRY PROGRESS  (3 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 6.1 Get progress history
curl -k -X GET https://localhost:7256/api/enquiryprogress/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 6.2 Add progress
curl -k -X POST https://localhost:7256/api/enquiryprogress/enquiry/{{ENQUIRY_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"status\":\"SITE_VISITED\",\"notes\":\"Visited site and took measurements\"}"

## 6.3 Update status with progress
curl -k -X POST https://localhost:7256/api/enquiryprogress/enquiry/{{ENQUIRY_ID}}/update-status ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"status\":\"MEASUREMENT_DONE\",\"notes\":\"All measurements completed\"}"


# ─────────────────────────────────────────────────────────────────────────────
# 7. MEASUREMENT  (7 endpoints)
#    First get a {{CATEGORY_ID}} from your MeasurementCategories table in SSMS
# ─────────────────────────────────────────────────────────────────────────────

## 7.1 Get measurements for enquiry
curl -k -X GET https://localhost:7256/api/measurement/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 7.2 Create measurement  → copy returned "id" as {{MEASUREMENT_ID}}
curl -k -X POST https://localhost:7256/api/measurement/{{ENQUIRY_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"categoryId\":\"{{CATEGORY_ID}}\",\"measurementData\":{\"length\":3.5,\"width\":2.0},\"notes\":\"Kitchen countertop main section\"}"

## 7.3 Get measurement by ID
curl -k -X GET https://localhost:7256/api/measurement/{{MEASUREMENT_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 7.4 Update measurement
curl -k -X PUT https://localhost:7256/api/measurement/{{MEASUREMENT_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"measurementData\":{\"length\":4.0,\"width\":2.5},\"notes\":\"Corrected after re-check\"}"

## 7.5 Delete measurement
curl -k -X DELETE https://localhost:7256/api/measurement/{{MEASUREMENT_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 7.6 Convert (generic)
curl -k -X POST https://localhost:7256/api/measurement/convert ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"type\":\"meterToSqFt\",\"length\":10,\"breadth\":5}"

## 7.7 Convert meter to sqft
curl -k -X POST https://localhost:7256/api/measurement/convert/meter-to-sqft ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"length\":3.5,\"breadth\":2.0}"


# ─────────────────────────────────────────────────────────────────────────────
# 8. QUOTATION  (8 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 8.1 Get quotations for enquiry
curl -k -X GET https://localhost:7256/api/quotation/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 8.2 Create quotation  → copy returned "id" as {{QUOTE_ID}}
curl -k -X POST https://localhost:7256/api/quotation ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"enquiryId\":\"{{ENQUIRY_ID}}\",\"quotationNumber\":\"QT-2026-0001\",\"taxPercentage\":18.0,\"notes\":\"Granite countertop installation quote\"}"

## 8.3 Get quotation by ID
curl -k -X GET https://localhost:7256/api/quotation/{{QUOTE_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 8.4 Update quotation
curl -k -X PUT https://localhost:7256/api/quotation/{{QUOTE_ID}} ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -d "{\"taxPercentage\":12.0,\"notes\":\"GST reduced to 12 percent\",\"status\":\"FINAL\"}"

## 8.5 Generate PDF
curl -k -X GET https://localhost:7256/api/quotation/{{QUOTE_ID}}/pdf ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  --output quotation.pdf

## 8.6 Download saved PDF
curl -k -X GET https://localhost:7256/api/quotation/{{QUOTE_ID}}/download-pdf ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  --output quotation_saved.pdf

## 8.7 Send quotation (mark as sent)
curl -k -X POST https://localhost:7256/api/quotation/{{QUOTE_ID}}/send ^
  -H "Authorization: Bearer {{TOKEN}}"

## 8.8 Delete quotation
curl -k -X DELETE https://localhost:7256/api/quotation/{{QUOTE_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 9. FILE UPLOAD  (5 endpoints)
# ─────────────────────────────────────────────────────────────────────────────

## 9.1 Upload file (replace path after -F)  → copy returned "id" as {{FILE_ID}}
curl -k -X POST "https://localhost:7256/api/file/upload/{{ENQUIRY_ID}}?category=SITE_PHOTO" ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  -F "file=@C:\path\to\photo.jpg"

## 9.2 Get files for enquiry
curl -k -X GET https://localhost:7256/api/file/enquiry/{{ENQUIRY_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 9.3 Get file by ID
curl -k -X GET https://localhost:7256/api/file/{{FILE_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"

## 9.4 Download file
curl -k -X GET https://localhost:7256/api/file/download/{{FILE_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}" ^
  --output downloaded_file.jpg

## 9.5 Delete file
curl -k -X DELETE https://localhost:7256/api/file/{{FILE_ID}} ^
  -H "Authorization: Bearer {{TOKEN}}"


# ─────────────────────────────────────────────────────────────────────────────
# 10. STAFF LOGIN – verify role restrictions
# ─────────────────────────────────────────────────────────────────────────────

## 10.1 Login as staff  → copy token as {{STAFF_TOKEN}}
curl -k -X POST https://localhost:7256/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"john@salesquotation.com\",\"password\":\"Staff@123\"}"

## 10.2 Staff CAN list own enquiries (200 OK)
curl -k -X GET https://localhost:7256/api/enquiry ^
  -H "Authorization: Bearer {{STAFF_TOKEN}}"

## 10.3 Staff BLOCKED from admin endpoints (403 Forbidden)
curl -k -X GET https://localhost:7256/api/staff ^
  -H "Authorization: Bearer {{STAFF_TOKEN}}"

## 10.4 Staff BLOCKED from creating materials (403 Forbidden)
curl -k -X POST https://localhost:7256/api/material ^
  -H "Content-Type: application/json" ^
  -H "Authorization: Bearer {{STAFF_TOKEN}}" ^
  -d "{\"name\":\"Test\",\"unit\":\"sqft\",\"baseCost\":10}"
