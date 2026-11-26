# API Contract: Car Show Registration System

Written by the Backend Developer (Justine Cacal) for the Frontend Developer (Nhel Nacional).
All examples below were taken from the running backend.

## How to run the backend

```
cd backend
dotnet run --project Api
```

- Base URL: `http://localhost:5205`
- Scalar docs (try every endpoint): `http://localhost:5205/scalar/v1`
- CORS allows the React dev server at `http://localhost:5173` only. If Vite uses another port, tell the Backend Developer.
- The database starts with 25 sample registrations, so the dashboard has data right away.

## Rules the frontend must know

| Rule | Detail |
|---|---|
| Enums are strings | Send and read `"Competitor"`, never a number |
| Registration types | `Exhibitor` (fee 1500), `Competitor` (fee 2500), `VIP` (fee 5000) |
| Statuses | `Pending`, `Confirmed`, `CheckedIn`, `Cancelled` |
| Payment statuses | `Unpaid`, `Partial`, `Paid` (computed by the server, never sent by the client) |
| Server sets these | `id`, `registrationNumber` (CS27-0001), `registrationDate`, `registrationFee`, `status` (always `Pending` on create), `paymentStatus`. The server ignores them if the client sends them. |
| Dates | UTC ISO 8601 ending in `Z`, for example `2026-11-19T10:55:36.9436914Z`. Convert to local time in the browser with `new Date(value)`. |
| Money | JSON numbers, for example `2500.0`. Format them in the UI (for example with `toLocaleString`). |
| Plate number | Must be unique (case-insensitive). A duplicate returns 409. |
| Endpoints | Only create, list, get by id and summary. There is no update or delete. |

## 1. POST /api/registrations

Request body (JSON):

```json
{
  "participantName": "Juan Dela Cruz",
  "contactNumber": "09171234567",
  "email": "juan.delacruz@example.com",
  "address": "123 Rizal St., Naga City",
  "carMakeModel": "Toyota Supra MK4",
  "plateNumber": "TST 1001",
  "registrationType": "Competitor",
  "amountPaid": 1000
}
```

Field rules:

| Field | Rule |
|---|---|
| participantName | required, up to 150 characters |
| contactNumber | required, exactly `09` followed by 9 digits (11 digits) |
| email | required, valid email, up to 200 characters |
| address | required, up to 300 characters |
| carMakeModel | required, up to 100 characters |
| plateNumber | required, up to 15 characters, unique |
| registrationType | required: `Exhibitor`, `Competitor` or `VIP` |
| amountPaid | required, from 0 up to the fee of the chosen type |

Success: `201 Created`, with a `Location: /api/registrations/{id}` header and this body:

```json
{
  "id": "71ca9c79-12e5-44f1-8246-9d908309d8f0",
  "registrationNumber": "CS27-0026",
  "registrationDate": "2026-11-19T10:55:36.9436914Z",
  "participantName": "Juan Dela Cruz",
  "contactNumber": "09171234567",
  "email": "juan.delacruz@example.com",
  "address": "123 Rizal St., Naga City",
  "carMakeModel": "Toyota Supra MK4",
  "plateNumber": "TST 1001",
  "registrationType": "Competitor",
  "status": "Pending",
  "registrationFee": 2500,
  "amountPaid": 1000.0,
  "paymentStatus": "Partial"
}
```

Errors:

400 validation error (`Content-Type: application/problem+json`). The keys inside `errors` are the field names in **PascalCase** (`ParticipantName`, not `participantName`). Map them to your form fields in the frontend.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation.Failed",
  "status": 400,
  "errors": {
    "ContactNumber": [
      "'Contact Number' must be a Philippine mobile number in the format 09XXXXXXXXX (11 digits)."
    ]
  },
  "traceId": "00-..."
}
```

409 duplicate plate number:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Registration.Conflict",
  "status": 409,
  "detail": "A registration with plate number 'TST 1001' already exists.",
  "traceId": "00-..."
}
```

A registration type that is not one of the three values (for example `"Sponsor"`) also returns 400.

## 2. GET /api/registrations

Query string (all optional):

| Parameter | Meaning | Default | Rule |
|---|---|---|---|
| participantName | name contains this text (ignores upper/lower case) | none | up to 150 characters |
| type | `Exhibitor`, `Competitor` or `VIP` | none | other values return 400 |
| status | `Pending`, `Confirmed`, `CheckedIn` or `Cancelled` | none | other values return 400 |
| page | page number | 1 | 1 or more |
| pageSize | rows per page | 10 | 1 to 50 |

Filters can be combined. Results are sorted newest first.

Example: `GET /api/registrations?type=VIP&pageSize=2`

```json
{
  "items": [
    {
      "id": "736cb257-a142-47de-87ab-e3a73e3dc6ee",
      "registrationNumber": "CS27-0029",
      "registrationDate": "2026-11-19T15:31:14.6888647Z",
      "participantName": "Juan Dela Cruz",
      "contactNumber": "09171234567",
      "email": "juan.delacruz@example.com",
      "address": "123 Rizal St., Naga City",
      "carMakeModel": "Toyota Supra MK4",
      "plateNumber": "TST 1005",
      "registrationType": "VIP",
      "status": "Pending",
      "registrationFee": 5000.0,
      "amountPaid": 0.0,
      "paymentStatus": "Unpaid"
    }
  ],
  "page": 1,
  "pageSize": 2,
  "totalCount": 8,
  "totalPages": 4
}
```

(Only one item is shown here to keep the example short. Each item has the same 14 fields as the POST response.)

- No matches: `200` with `"items": []`, `"totalCount": 0`, `"totalPages": 0`. This is not an error, show an empty-state message.
- A page past the end: `200` with `"items": []` and the real `totalCount`.
- `page=0` or `pageSize=100`: `400` with `errors.Page` or `errors.PageSize`. Keep the page controls inside these limits so the user never sees this error.

Use `totalPages` for the pager and `totalCount` for "Showing X of Y".

## 3. GET /api/registrations/{id}

- Found: `200`, one object with the same 14 fields as the POST response.
- Not found: `404`, title `Registration.NotFound`, and a `detail` message.
- An id that is not a valid GUID (for example `/api/registrations/abc`) returns `404` with an empty body.

## 4. GET /api/registrations/summary

Returns the KPI numbers for the dashboard cards. They are calculated by the server over **all** registrations, not only the current page.

```json
{
  "totalRegistrations": 30,
  "confirmed": 10,
  "checkedIn": 6,
  "pending": 11,
  "cancelled": 3,
  "totalFees": 77000.0,
  "totalPayments": 48500.0,
  "uncollectedPayments": 28500.0
}
```

- `totalRegistrations` = `confirmed` + `checkedIn` + `pending` + `cancelled`.
- `totalFees`, `totalPayments` and `uncollectedPayments` **do not include Cancelled** registrations.
- `uncollectedPayments` = `totalFees` - `totalPayments`.
- Call this endpoint again after every successful create, so the cards stay correct.

## Integration checklist for the frontend

1. The dashboard shows data from `GET /api/registrations` and `GET /api/registrations/summary`.
2. The form posts the 8 fields above and shows the 400 field errors next to the matching inputs.
3. A 409 shows a clear message about the plate number.
4. Dates are shown in local time and money is formatted.
5. Filters and paging send query-string values and use `totalPages` for the pager.
