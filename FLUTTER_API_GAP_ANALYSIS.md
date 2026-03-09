# Flutter ↔ Backend API — Gap Analysis

## Backend Actual Endpoints (what's running)

| # | Method | Actual Backend Route | Role |
|---|--------|---------------------|------|
| 1 | POST | `/api/auth/login` | Any |
| 2 | POST | `/api/auth/logout` | Auth |
| 3 | GET | `/api/enquiry` | Auth (Admin=all, Staff=own) |
| 4 | GET | `/api/enquiry/{id}` | Auth |
| 5 | POST | `/api/enquiry` | Auth |
| 6 | PUT | `/api/enquiry/{id}` | Auth |
| 7 | DELETE | `/api/enquiry/{id}` | Admin |
| 8 | GET | `/api/enquiryprogress/enquiry/{enquiryId}` | Auth |
| 9 | POST | `/api/enquiryprogress/enquiry/{enquiryId}` | Auth |
| 10 | POST | `/api/enquiryprogress/enquiry/{enquiryId}/update-status` | Auth |
| 11 | GET | `/api/enquirystatusconfig` | Auth |
| 12 | GET | `/api/enquirystatusconfig/{id}` | Auth |
| 13 | POST | `/api/enquirystatusconfig` | Admin |
| 14 | PUT | `/api/enquirystatusconfig/{id}` | Admin |
| 15 | DELETE | `/api/enquirystatusconfig/{id}` | Admin |
| 16 | GET | `/api/staff` | Admin |
| 17 | GET | `/api/staff/{id}` | Admin |
| 18 | POST | `/api/staff` | Admin |
| 19 | PUT | `/api/staff/{id}` | Admin |
| 20 | DELETE | `/api/staff/{id}` | Admin |
| 21 | POST | `/api/staff/assign-enquiry` | Admin |
| 22 | PUT | `/api/staff/{id}/role` | Admin (feature-flagged) |
| 23 | GET | `/api/material` | Auth |
| 24 | GET | `/api/material/{id}` | Auth |
| 25 | POST | `/api/material` | Admin |
| 26 | PUT | `/api/material/{id}` | Admin |
| 27 | DELETE | `/api/material/{id}` | Admin |
| 28 | GET | `/api/measurement/enquiry/{enquiryId}` | Auth |
| 29 | GET | `/api/measurement/{id}` | Auth |
| 30 | POST | `/api/measurement/{enquiryId}` | Auth |
| 31 | PUT | `/api/measurement/{id}` | Auth |
| 32 | DELETE | `/api/measurement/{id}` | Auth |
| 33 | POST | `/api/measurement/convert` | Auth |
| 34 | POST | `/api/measurement/convert/meter-to-sqft` | Auth |
| 35 | GET | `/api/quotation/enquiry/{enquiryId}` | Auth |
| 36 | GET | `/api/quotation/{id}` | Auth |
| 37 | POST | `/api/quotation` | Auth |
| 38 | PUT | `/api/quotation/{id}` | Auth |
| 39 | DELETE | `/api/quotation/{id}` | Auth |
| 40 | GET | `/api/quotation/{id}/pdf` | Auth |
| 41 | GET | `/api/quotation/{id}/download-pdf` | Auth |
| 42 | POST | `/api/quotation/{id}/send` | Auth |
| 43 | POST | `/api/file/upload/{enquiryId}?category=X` | Auth |
| 44 | GET | `/api/file/enquiry/{enquiryId}` | Auth |
| 45 | GET | `/api/file/{id}` | Auth |
| 46 | GET | `/api/file/download/{id}` | Auth |
| 47 | DELETE | `/api/file/{id}` | Auth |

---

## Mismatches Between Flutter Frontend & Backend

### 🔴 CRITICAL — URL Mismatches (Flutter calls will 404)

| # | Flutter Expects | Backend Actually Has | Fix Needed |
|---|----------------|---------------------|------------|
| 1 | Base URL `localhost:5000` | `localhost:5132` (http) or `localhost:7256` (https) | Change base URL in Flutter |
| 2 | `GET /api/staff/enquiries` | `GET /api/enquiry` | Change Flutter API path |
| 3 | `POST /api/staff/enquiries` | `POST /api/enquiry` | Change Flutter API path |
| 4 | `GET /api/staff/enquiries/:id` | `GET /api/enquiry/{id}` | Change Flutter API path |
| 5 | `PUT /api/staff/enquiries/:id/status` | `POST /api/enquiryprogress/enquiry/{id}/update-status` | Change Flutter path + method + body |
| 6 | `GET /api/admin/enquiries` | `GET /api/enquiry` (same endpoint, role-based) | Change Flutter API path |
| 7 | `/api/admin/staff/...` | `/api/staff` | Change Flutter API path |
| 8 | `/api/admin/materials/...` | `/api/material` | Change Flutter API path |

### 🟡 Data Format Mismatches

| # | Issue | Flutter Expects | Backend Sends |
|---|-------|----------------|---------------|
| 1 | **Role casing** | `"ADMIN"`, `"STAFF"` | `"Admin"`, `"Staff"` (PascalCase) |
| 2 | **Field naming** | Both `camelCase` and `snake_case` | `camelCase` only |
| 3 | **Pagination** | `?page=1&limit=10` query params | Not implemented (returns all) |
| 4 | **Status update body** | `{ "new_status": "...", "comment": "..." }` | `{ "status": "...", "notes": "..." }` |

### 🟢 Flutter Modules Not Yet Implemented (need full build)

| Module | Backend Endpoints Ready | Flutter Status |
|--------|------------------------|----------------|
| **Measurement CRUD** | 7 endpoints at `/api/measurement/...` | Model referenced, no UI/API code |
| **Quotation CRUD + PDF** | 8 endpoints at `/api/quotation/...` | No code |
| **Enquiry Progress/History** | 3 endpoints at `/api/enquiryprogress/...` | No code |
| **Enquiry Status Config** | 5 endpoints at `/api/enquirystatusconfig/...` | No code |
| **File Upload/Download** | 5 endpoints at `/api/file/...` | Dependencies added, no code |
| **Role Management** | `PUT /api/staff/{id}/role` | No code |
| **Measurement Conversion** | 2 endpoints at `/api/measurement/convert/...` | No code |
| **Logout endpoint** | `POST /api/auth/logout` | Only local clear (OK, but could call backend) |

---

## Backend Response Format Reference

All responses follow this envelope:

```json
// Success
{
  "success": true,
  "data": { ... }
}

// Error
{
  "success": false,
  "error": "Error message",
  "code": "ERROR_CODE"
}
```

Error codes: `UNAUTHORIZED`, `FORBIDDEN`, `NOT_FOUND`, `BAD_REQUEST`, `INVALID_ROLE`, `FEATURE_DISABLED`, `INTERNAL_ERROR`
