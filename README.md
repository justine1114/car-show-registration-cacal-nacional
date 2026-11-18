# Car Show Registration System

Integrative Programming 2 (API Backend Development), Final Project.
Registration system for **Car Show 2027** (AutoVista Events), made of a C# Vertical Slice API and a React dashboard.

## Team

| MEMBER | ROLE |
| --- | --- |
| Justine Cacal | Backend Developer |
| Nhel Nacional | Frontend Developer |

## Repository layout

```
car-show-registration/
|-- backend/      C# .NET 10 Vertical Slice API (Justine)
|-- frontend/     React (Vite) dashboard (Nhel)
|-- docs/
|   `-- screenshots/
`-- README.md
```

## Tech stack

| Part | Technology |
| --- | --- |
| Backend | C#, .NET 10, EF Core, SQLite, FluentValidation, Scalar |
| Frontend | React (Vite), Tailwind CSS or Bootstrap |

## How to run the backend

Requires the .NET 10 SDK.

```
cd backend
dotnet run --project Api
```

- API base URL: http://localhost:5205
- Scalar API docs: http://localhost:5205/scalar/v1

## How to run the frontend

See `frontend/README.md` (added by the Frontend Developer).

## Git workflow

- `main` holds the final, working code for both parts.
- Backend work is done on the `backend` branch, frontend work on the `frontend` branch.
- Changes are merged into `main` through a Pull Request reviewed by the other member.
